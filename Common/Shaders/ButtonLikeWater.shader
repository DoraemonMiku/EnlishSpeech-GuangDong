// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UIFrostedGlass"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_NormalTex("Sprite Texture", 2D) = "white" {}
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Stencil
			{
				Ref[_Stencil]
				Comp[_StencilComp]
				Pass[_StencilOp]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
			}

			Cull Off
			Lighting Off
			ZWrite Off
			ZTest[unity_GUIZTestMode]
			Blend SrcAlpha OneMinusSrcAlpha
			GrabPass{ }
			Pass
			{
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"
				#include "UnityUI.cginc"

				struct appdata_t
				{
					float4 vertex   : POSITION;
					float4 color    : COLOR;
					float2 texcoord : TEXCOORD0;
				};
				struct v2f
				{
					float4 vertex   : SV_POSITION;
					half2 texcoord  : TEXCOORD0;
					float4 worldPosition : TEXCOORD1;
				};

				v2f vert(appdata_t IN)
				{
					v2f OUT;
					OUT.worldPosition = IN.vertex;
					OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
					OUT.texcoord = half2(IN.texcoord.x, 1 - IN.texcoord.y);
					return OUT;
				}

				sampler2D _MainTex;
				uniform half4 _MainTex_TexelSize;
				sampler2D _GrabTexture;
				uniform half4 _GrabTexture_TexelSize;
				sampler2D _NormalTex;
				float _RandomParam;
				static const half4 curve4[7] = { 	half4(0.0205,0.0205,0.0205,0),
													half4(0.0855,0.0855,0.0855,0),
													half4(0.232,0.232,0.232,0),
													half4(0.324,0.324,0.324,1),
													half4(0.232,0.232,0.232,0),
													half4(0.0855,0.0855,0.0855,0),
													half4(0.0205,0.0205,0.0205,0) };
				fixed4 frag(v2f IN) : SV_Target
				{
					half4 nor = tex2D(_NormalTex, IN.texcoord * 8);
					half4 nor2 = tex2D(_NormalTex, IN.texcoord * 12);
					half4 nor3 = tex2D(_NormalTex, IN.texcoord * 23);
					half4 nor4 = tex2D(_NormalTex, IN.texcoord * 26);
					half4 nor5 = tex2D(_NormalTex, IN.texcoord * 33);
					half4 nor6 = tex2D(_NormalTex, IN.texcoord * 41);
					half4 nor7 = tex2D(_NormalTex, IN.texcoord * 51);
					half4 nor8 = tex2D(_NormalTex, IN.texcoord * 62);
					half4 nor9 = tex2D(_NormalTex, IN.texcoord * 73);
					half4 nor10 = tex2D(_NormalTex, IN.texcoord * 86);
					half4 nor11 = tex2D(_NormalTex, IN.texcoord * 93);
					half4 nor12 = tex2D(_NormalTex, IN.texcoord * 91);


					half4 norcolor1 = tex2D(_GrabTexture, IN.texcoord + (20 * _GrabTexture_TexelSize * nor.xy));
					half4 norcolor2 = tex2D(_GrabTexture, IN.texcoord - (20 * _GrabTexture_TexelSize * nor.yz));
					half4 norcolor3 = tex2D(_GrabTexture, IN.texcoord + (20 * _GrabTexture_TexelSize * nor.zx));
					half4 norcolor21 = tex2D(_GrabTexture, IN.texcoord - (20 * _GrabTexture_TexelSize * nor2.xy));
					half4 norcolor22 = tex2D(_GrabTexture, IN.texcoord + (20 * _GrabTexture_TexelSize * nor2.yz));
					half4 norcolor23 = tex2D(_GrabTexture, IN.texcoord - (20 * _GrabTexture_TexelSize * nor2.zx));
					half4 norcolor31 = tex2D(_GrabTexture, IN.texcoord + (20 * _GrabTexture_TexelSize * nor3.xy));
					half4 norcolor32 = tex2D(_GrabTexture, IN.texcoord - (20 * _GrabTexture_TexelSize * nor3.yz));
					half4 norcolor33 = tex2D(_GrabTexture, IN.texcoord + (20 * _GrabTexture_TexelSize * nor3.zx));
					half4 norcolor41 = tex2D(_GrabTexture, IN.texcoord - (20 * _GrabTexture_TexelSize * nor4.xy));
					half4 norcolor42 = tex2D(_GrabTexture, IN.texcoord + (30 * _GrabTexture_TexelSize * nor4.yz));
					half4 norcolor43 = tex2D(_GrabTexture, IN.texcoord - (30 * _GrabTexture_TexelSize * nor4.zx));
					half4 norcolor51 = tex2D(_GrabTexture, IN.texcoord + (30 * _GrabTexture_TexelSize * nor5.xy));
					half4 norcolor52 = tex2D(_GrabTexture, IN.texcoord - (30 * _GrabTexture_TexelSize * nor5.yz));
					half4 norcolor53 = tex2D(_GrabTexture, IN.texcoord + (30 * _GrabTexture_TexelSize * nor5.zx));
					half4 norcolor61 = tex2D(_GrabTexture, IN.texcoord - (30 * _GrabTexture_TexelSize * nor6.xy));
					half4 norcolor62 = tex2D(_GrabTexture, IN.texcoord + (40 * _GrabTexture_TexelSize * nor6.yz));
					half4 norcolor63 = tex2D(_GrabTexture, IN.texcoord - (40 * _GrabTexture_TexelSize * nor6.zx));
					half4 norcolor71 = tex2D(_GrabTexture, IN.texcoord + (40 * _GrabTexture_TexelSize * nor7.xy));
					half4 norcolor72 = tex2D(_GrabTexture, IN.texcoord - (40 * _GrabTexture_TexelSize * nor7.yz));
					half4 norcolor73 = tex2D(_GrabTexture, IN.texcoord + (40 * _GrabTexture_TexelSize * nor7.zx));
					half4 norcolor81 = tex2D(_GrabTexture, IN.texcoord - (50 * _GrabTexture_TexelSize * nor8.xy));
					half4 norcolor82 = tex2D(_GrabTexture, IN.texcoord + (50 * _GrabTexture_TexelSize * nor8.yz));
					half4 norcolor83 = tex2D(_GrabTexture, IN.texcoord - (50 * _GrabTexture_TexelSize * nor8.zx));
					half4 norcolor91 = tex2D(_GrabTexture, IN.texcoord + (50 * _GrabTexture_TexelSize * nor9.xy));
					half4 norcolor92 = tex2D(_GrabTexture, IN.texcoord - (60 * _GrabTexture_TexelSize * nor9.yz));
					half4 norcolor93 = tex2D(_GrabTexture, IN.texcoord + (60 * _GrabTexture_TexelSize * nor9.zx));
					half4 norcolor101 = tex2D(_GrabTexture, IN.texcoord - (60 * _GrabTexture_TexelSize * nor10.xy));
					half4 norcolor102 = tex2D(_GrabTexture, IN.texcoord + (70 * _GrabTexture_TexelSize * nor10.yz));
					half4 norcolor103 = tex2D(_GrabTexture, IN.texcoord - (70 * _GrabTexture_TexelSize * nor10.zx));
					half4 norcolor111 = tex2D(_GrabTexture, IN.texcoord + (80 * _GrabTexture_TexelSize * nor11.xy));
					half4 norcolor112 = tex2D(_GrabTexture, IN.texcoord - (80 * _GrabTexture_TexelSize * nor11.yz));
					half4 norcolor113 = tex2D(_GrabTexture, IN.texcoord + (90 * _GrabTexture_TexelSize * nor11.zx));
					half4 norcolor121 = tex2D(_GrabTexture, IN.texcoord - (90 * _GrabTexture_TexelSize * nor12.xy));
					half4 norcolor122 = tex2D(_GrabTexture, IN.texcoord + (100 * _GrabTexture_TexelSize * nor12.yz));
					half4 norcolor123 = tex2D(_GrabTexture, IN.texcoord - (110 * _GrabTexture_TexelSize * nor12.zx));
					half4 color = (norcolor1 + norcolor2 + norcolor3
					+ norcolor21 + norcolor22 + norcolor23
					+ norcolor31 + norcolor32 + norcolor33
					+ norcolor41 + norcolor42 + norcolor43
					+ norcolor51 + norcolor52 + norcolor53
					+ norcolor61 + norcolor62 + norcolor63
					+ norcolor71 + norcolor72 + norcolor73
					+ norcolor81 + norcolor82 + norcolor83
					+ norcolor91 + norcolor92 + norcolor93
					+ norcolor101 + norcolor102 + norcolor103
					+ norcolor111 + norcolor112 + norcolor113
					+ norcolor121 + norcolor122 + norcolor123) / 36;
					return color;
				}
			ENDCG
			}
		}
}