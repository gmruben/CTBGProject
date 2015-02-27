Shader "Custom/FontColorShadow"
{
    Properties
	{
		_Tex ("Texture", 2D) = "white" {}
		_Outline ("Outline", Float) = 0.01
    }
    
	SubShader 
    {
       Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
   		
   		Lighting Off ZWrite Off Fog { Mode Off }
   		Blend SrcAlpha OneMinusSrcAlpha 
        
		Pass
		{
			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _Tex;
			float _Outline;

			float4 _Tex_ST;
			
            struct VertOut
            {
                float4 position : POSITION;
                float4 color : COLOR;
				float2 uv : TEXCOORD0;
            };
 
            struct VertIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
            };
 
            VertOut vert(VertIn input)
            {
                VertOut output;

                output.position = mul(UNITY_MATRIX_MVP,input.vertex);
				output.uv = TRANSFORM_TEX(input.texcoord, _Tex);
				output.color = input.color;

				output.position.x += _Outline;
				output.position.y += -_Outline;
				output.color = float4(0, 0, 0, 1);

                return output;
            }
 
            fixed4 frag (VertOut output) : COLOR
			{
				return fixed4(output.color.rgb, tex2D(_Tex, output.uv.xy).a * 0.5);
			}

            ENDCG
		}

		Pass
        {   	
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _Tex;
			float4 _Tex_ST;
 
            struct VertOut
            {
                float4 position : POSITION;
                float4 color : COLOR;
				float2 uv : TEXCOORD0;
            };
 
            struct VertIn
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
            };
 
            VertOut vert(VertIn input)
            {
                VertOut output;

                output.position = mul(UNITY_MATRIX_MVP,input.vertex);
				output.uv = TRANSFORM_TEX(input.texcoord, _Tex);
				output.color = input.color;

                return output;
            }
 
            fixed4 frag (VertOut output) : COLOR
			{
				return fixed4(output.color.rgb, tex2D(_Tex, output.uv.xy).a);
			}

            ENDCG
        }
    }
    FallBack "Diffuse"
}