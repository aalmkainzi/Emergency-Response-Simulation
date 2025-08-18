Shader "Unlit/CombinedShader"
{
        Properties
    {
        //Oil Gradient
        _Offset ("Offset", Vector) = (0.1,0.1,0,0)
        _NoiseScale ("Noise Scale", Float) = 1
        _FirstColor ("First Color", Color) = (.8,0,1,1)
        _SecondColor ("Second Color", Color) = (.25, 0, 1, 1)
        _ThirdColor ("Third Color", Color) = (.9, 1, .07, 1)
        _FourthColor ("Fourth Color", Color) = (0, 1, 0.35, 1)
        _FifthColor ("Fifth Color", Color) = (0.7686275, 0.5301423, 0.3490196, 1)
        _SixthColor ("Sixth Color", Color) = (0, 0.4865584, 1, 1)
        _Alpha ("Alpha", Float) = 1
        _Threshold ("Threshold", Float) = 0.5
        //screen depth
        _DepthMaxDistance ("Max Depth", Float) = 1
        //Foam
        _FoamDistance ("Foam Distance", Float) = 1
        _FoamCutoff ("Foam Cutoff", Float) = 1
        _FoamColor ("Foam Color", Color) = (1,1,1,1)
        _FoamNoise ("Foam Noise", Float) = 1
        //Displacement
        _DisplacementStrength ("Displacement Strength", Float) = .5
        _Radius ("Radius", Float) = .5
        //Normal Map
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Range(0, 1)) = 1
        //Mask Map
        _MaskMap ("Mask Map", 2D) = "white" {}
        //PRDF
        _Smoothness ("Smoothness", Float) = 1
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
        _Tint ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityPBSLighting.cginc"

            #define RANDOM1 12.9898
            #define RANDOM2 78.233
            #define RANDOM3 43758.54531213

            sampler2D _MaskMap;
            float4 _MaskMap_ST;
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
                float3 worldPos : TEXCOORD5;
            };

            float random(float2 st)
            {
                float randomVal = frac(sin(dot(st.xy, float2(RANDOM1, RANDOM2))) * RANDOM3);
                return  randomVal;
            }

            float simple_Noise(float2 st)
            {
                float2 i = floor(st);
                float2 f = frac(st);

                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            float4 _FirstColor, _SecondColor, _ThirdColor, _FourthColor,
            _FifthColor, _SixthColor, _FoamColor, _Tint;
            float2 _Offset;
            float _NoiseScale, _Alpha, _Threshold, _DepthMaxDistance, _FoamDistance, _Metallic,
            _FoamCutoff, _FoamNoise, _DisplacementStrength, _Radius, _BumpScale, _Smoothness;
            sampler2D _CameraDepthTexture, _NormalMap;
            
            v2f vert (appdata v)
            {
                v2f o;
                float3 mask = 1 - tex2Dlod(_MaskMap, float4(v.uv.xy, 0,0));
                
                float2 centereedUV = (v.uv - .5) * 2;
                float distance = dot(centereedUV, centereedUV);
                float distplacement = (simple_Noise(_Radius * centereedUV) - distance) * _DisplacementStrength * mask;
                float3 displacementPosition = v.vertex.xyz + (v.normal * distplacement);
                o.vertex = UnityObjectToClipPos(displacementPosition);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = centereedUV;
                o.tangent = UnityObjectToWorldDir(v.tangent);
                o.bitangent = cross(o.normal, o.tangent);
                o.bitangent = o.bitangent * v.tangent.w * unity_WorldTransformParams.w;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            void InitializeNormal(inout v2f i)
            {
                float3 normalMapped = UnpackNormal(tex2D(_NormalMap, i.uv));
                normalMapped = normalize(lerp(float3(0, 0, 1), normalMapped, _BumpScale));

                float3x3 tangentToWorld =
                {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };
                i.normal = normalize(mul(tangentToWorld, normalMapped));
            }
            
            UnityLight createLight(v2f i)
            {
                UnityLight directLight;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                directLight.color = _LightColor0.rbg;
                directLight.ndotl = DotClamped(i.normal, lightDir);
                directLight.dir = lightDir;
                return directLight;
            }
            
            UnityIndirect createIndirect(v2f i)
            {
                UnityIndirect indirectLight;
                indirectLight.diffuse = .1;
                indirectLight.specular = 0;
                return indirectLight;
            }
            float4 frag (v2f i) : SV_Target
            {
                //prdf
                InitializeNormal(i);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 specTint;
                float oneMinusReflect;
                

                //
                float3 diffuse = DotClamped(i.normal, float3(1,1,.5));
                //UVs and Noise
                float2 centered = abs(i.uv);
                float2 offseted = centered - _Offset;
                float2 clamped = saturate(offseted);
                float ed = length(clamped);
                float noise = simple_Noise(offseted * _NoiseScale);
                //Fixed Alpha
                float fixedAlpha = _Alpha / 5;
                clip(1 - (noise * ed) + _Threshold);

                //cut UVs and lerps
                float4 firstLerp = lerp(_FirstColor, _SecondColor, ed);
                float4 secondLerp = lerp(firstLerp, _ThirdColor, ed);
                float4 thirdLerp = lerp(secondLerp, _FourthColor, ed);
                float4 fourthLerp = lerp(thirdLerp, _FifthColor, ed);
                float4 fifthLerp = lerp(fourthLerp, _SixthColor, ed);

                //ScreenDepth
                float2 screenUVs = i.screenPos.xy / i.screenPos.w;
                float existingDepth = tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos.xy) / i.screenPos.w);
                float existingDepthLinear = LinearEyeDepth(existingDepth);
                float depthDiff = existingDepthLinear - i.screenPos.w;
                float oilDepthDiff = saturate(depthDiff / _DepthMaxDistance);

                //foam
                float foamDepthDiff = saturate(depthDiff / _FoamDistance);
                float cutOff = foamDepthDiff * _FoamCutoff;
                float foamNoise = simple_Noise(screenUVs * _FoamNoise);
                float foamStep = step(cutOff, foamNoise);
                float4 foamFinal = foamStep * _FoamColor.rgba;

                //float3 finalDiffuse =  fifthLerp.rgb;
                //float3 foamAndOil = float3(finalDiffuse + foamFinal.rgb);
                //prdf cont
                float3 finalTint = fifthLerp.rgb * _Tint.rgb;
                finalTint = DiffuseAndSpecularFromMetallic(finalTint, _Metallic, specTint, oneMinusReflect);
                float4 finalColor = UNITY_BRDF_PBS(finalTint, specTint, oneMinusReflect,
                    _Smoothness, i.normal, viewDir, createLight(i), createIndirect(i));
                finalColor.a = fixedAlpha;
                foamFinal.a = fixedAlpha;
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}