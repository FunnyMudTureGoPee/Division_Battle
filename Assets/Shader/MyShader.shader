Shader "Unlit/MyShader"
{
    Properties
    {
        _Density("Density",Range(0,1))=0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 包含 UnityObjectToWorldNormal helper 函数的 include 文件
            #include "UnityCG.cginc"

            struct v2f {
                // 我们将输出世界空间法线作为常规 ("texcoord") 插值器之一
                half3 worldNormal : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // 顶点着色器：将对象空间法线也作为输入
            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                // UnityCG.cginc 文件包含将法线从对象变换到世界空间的函数，请使用该函数
                o.worldNormal = UnityObjectToWorldNormal(normal);
                return o;
            }
            float _Density;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = 0;
                // 法线是具有 xyz 分量的 3D 矢量；处于 -1..1范围。要将其显示为颜色，请将此范围设置为 0..1并放入红色、绿色、蓝色分量
                c.rgb = i.worldNormal*0.5+_Density;
                return c;
            }
            ENDCG
        }
    }
}
