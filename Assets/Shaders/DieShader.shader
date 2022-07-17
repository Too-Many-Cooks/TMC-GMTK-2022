Shader "Custom/Dice"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpStrength ("Normal Strength", Range(0,1)) = 0.5
        _GlossMap ("Smoothness Map", 2D) = "white" {}
        _MetallicMap ("Metallic Map", 2D) = "black" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _GridSize ("Grid Size", Range(1,16)) = 4
        _Atlas ("Atlas", 2D) = "black" {}
        _Remap ("Remap", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma require 2darray

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _GlossMap;
        sampler2D _MetallicMap;
        sampler2D _Remap;
        sampler2D _Atlas;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float2 uv2_Atlas;
        };

        half _BumpStrength;
        half _Glossiness;
        half _Metallic;
        half _GridSize;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            const float2 uv = IN.uv_MainTex;
            const float2 uv2 = IN.uv2_Atlas;
            const fixed factor = 1.0 / _GridSize;
            const float2 grid = floor(uv2 * _GridSize) / _GridSize;
            const float2 face_uv = (uv2 - grid) / factor;
            half4 remap = tex2D (_Remap, float2(grid.x, 1.0 - grid.y - factor));
            
            fixed4 c1 = tex2D (_MainTex, uv) * _Color;
            fixed4 c2 = tex2D (_Atlas, remap.xy + remap.zw * face_uv);
            fixed3 normal = UnpackNormal(tex2D(_BumpMap, uv));
            normal.xy *= _BumpStrength;

            o.Albedo = lerp(c1, c2.rgb, c2.a);
            o.Normal = normal;
            o.Metallic = tex2D (_MetallicMap, uv) * _Metallic;
            o.Smoothness = tex2D (_GlossMap, uv) * _Glossiness;
            o.Alpha = c1.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
