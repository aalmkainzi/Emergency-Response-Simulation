Shader "Unlit/AFFFShader"
{
    Properties
    {
        //Main Texure
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        //Normal Map
        [NoScaleOffset] _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Float) = 1
        //Height Map
        [NoScaleOffset] _HeightMap ("Height Map", 2D) = "gray" {}
        _HeightDisplacement ("Height Displacement", Float) = 0.5
        //Light Stuff
        _Smoothness ("Smoothness", Float) = 0
        [Gamma] _Metallic ("Metallic", Float) = 0
        _Tint ("Tint", Color) = (1,1,1,1)
        //Fixes
        _Alpha ("Alpha", Range(0, 2)) = 1
        _StepValue ("Step Value", Vector) = (0,0,0,0)
        _HillRadius ("Hill Radius", Float) = 1
        _HillCenter ("Hill Center", Float) = 1
        _HillFalloff ("Hill Fall Off", Float) = 1
        _HillHeight ("Hill Height", Float) = 1
        _Radius ("Radius", Range(0, 1)) = .5
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
            #include "AutoLight.cginc"

            #define RANDOM1 12.9898
            #define RANDOM2 78.233
            #define RANDOM3 43758.54531213

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
                float3 worldPos : TEXCOORD2;
                float3 tangent : TEXCOORD3;
                float3 bitangent : TEXCOORD4;
            };

            sampler2D _MainTex, _NormalMap, _HeightMap;
            float4 _Color, _Tint, _StepValue;
            float _NormalIntensity, _HeightDisplacement, _Smoothness, _Metallic,
            _Alpha, _HillCenter, _HillRadius, _HillFalloff, _HillHeight, _Radius;

            void InitializeNormal(inout v2f i)
            {
                float3 normalMapped = UnpackNormal(tex2D(_NormalMap, i.uv));
                normalMapped = normalize(lerp(float3(0, 0, 1), normalMapped, _NormalIntensity));

                float3x3 tangentToWorld =
                {
                    i.tangent.x, i.bitangent.x, i.normal.x,
                    i.tangent.y, i.bitangent.y, i.normal.y,
                    i.tangent.z, i.bitangent.z, i.normal.z
                };
                i.normal = normalize(mul(tangentToWorld, normalMapped));
            }

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
                //return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }
            
            v2f vert (appdata v)
            {
                v2f o;

                float height = tex2Dlod(_HeightMap, float4(v.uv, 0, 0));
                float displacement = smoothstep(0.1, 0.6, height) * _HeightDisplacement;

                float3 pos = v.vertex.xyz;
                pos += v.normal * displacement;

                float2 hillCenter = _HillCenter;
                float dist = length(v.uv - hillCenter);
                float t = 1.0 - smoothstep(0.0, _HillRadius, dist);
                float hill = pow(t, _HillFalloff) * _HillHeight;
                pos += v.normal * hill;

                o.vertex = UnityObjectToClipPos(float4(pos, 1.0));
                o.uv = v.uv;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.tangent = UnityObjectToWorldDir(v.tangent);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w * unity_WorldTransformParams.w;
                o.worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0));
                return o;
            }

            
            UnityLight createLight(v2f i)
            {
                UnityLight directLight;
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                directLight.color = _LightColor0.rgb;
                directLight.ndotl = DotClamped(i.normal, lightDir);
                directLight.dir = lightDir;
                return directLight;
            }
            
            UnityIndirect createIndirect(v2f i)
            {
                UnityIndirect indirectLight;
                indirectLight.diffuse = 0.5;
                indirectLight.specular = 0.3;
                return indirectLight;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                InitializeNormal(i);

                //radius
                float dist = (distance(i.uv, float2(0.5, 0.5)));
                clip(_Radius - dist);
                
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 specTint;
                float oneMinusReflect;
                float4 albedo = tex2D(_MainTex, i.uv);
                float3 finalTint = albedo.rgb * _Tint.rgb;
                finalTint = DiffuseAndSpecularFromMetallic(finalTint, _Metallic, specTint, oneMinusReflect);
                float4 finalColor = UNITY_BRDF_PBS(finalTint, specTint, oneMinusReflect,
                    _Smoothness, i.normal, viewDir, createLight(i), createIndirect(i));
                finalColor.a = _Alpha;
                
                return finalColor * _Color;
            }
            ENDCG
        }
    }
}
