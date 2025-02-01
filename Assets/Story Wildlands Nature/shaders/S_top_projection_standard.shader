// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TidalFlask/Top Projection Standard"
{
	Properties
	{
		_BaseTexture("Base Texture", 2D) = "white" {}
		_BaseTextureTint("Base Texture Tint", Color) = (1,1,1,0)
		_BaseTextureSaturation("Base Texture Saturation", Range( 0 , 2)) = 0
		_TopTexture("Top Texture", 2D) = "white" {}
		_TopTextureTint("Top Texture Tint", Color) = (1,1,1,0)
		_TopTextureTiling("Top Texture Tiling", Float) = 1
		_CoverageAmount("Coverage Amount", Range( -1 , 1)) = -0.15
		_CoverageFalloff("Coverage Falloff", Range( 0.01 , 2)) = 0.5
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
		};

		uniform float4 _BaseTextureTint;
		uniform sampler2D _BaseTexture;
		uniform float4 _BaseTexture_ST;
		uniform float _BaseTextureSaturation;
		uniform float4 _TopTextureTint;
		uniform sampler2D _TopTexture;
		uniform float _TopTextureTiling;
		uniform float _CoverageAmount;
		uniform float _CoverageFalloff;
		uniform float _Smoothness;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_BaseTexture = i.uv_texcoord * _BaseTexture_ST.xy + _BaseTexture_ST.zw;
			float3 desaturateInitialColor131 = ( _BaseTextureTint * tex2D( _BaseTexture, uv_BaseTexture ) ).rgb;
			float desaturateDot131 = dot( desaturateInitialColor131, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar131 = lerp( desaturateInitialColor131, desaturateDot131.xxx, ( 1.0 - _BaseTextureSaturation ) );
			float3 ase_worldPos = i.worldPos;
			float3 break109 = ase_worldPos;
			float2 appendResult113 = (float2(break109.x , break109.z));
			float3 ase_worldNormal = i.worldNormal;
			float4 lerpResult62 = lerp( float4( desaturateVar131 , 0.0 ) , ( _TopTextureTint * tex2D( _TopTexture, (appendResult113*_TopTextureTiling + 0.0) ) ) , pow( saturate( ( ase_worldNormal.y + _CoverageAmount ) ) , _CoverageFalloff ));
			o.Albedo = lerpResult62.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
/*ASEBEGIN
Version=18909
171;914;1906;822;3526.035;898.8017;2.260406;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;91;-2297.292,152.0532;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;124;-2042.561,521.222;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;71;-2075.595,699.7294;Float;False;Property;_CoverageAmount;Coverage Amount;6;0;Create;True;0;0;0;False;0;False;-0.15;-0.18;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;109;-2051.46,148.7199;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-1718.484,521.9647;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-1970.71,300.1143;Inherit;False;Property;_TopTextureTiling;Top Texture Tiling;5;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;113;-1897.404,148.3374;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;134;-1688.072,151.4054;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;108;-1558.484,521.9647;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-1730.347,701.3966;Float;False;Property;_CoverageFalloff;Coverage Falloff;7;0;Create;True;0;0;0;False;0;False;0.5;0.65;0.01;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-1394.053,-375.6969;Inherit;True;Property;_BaseTexture;Base Texture;0;0;Create;True;0;0;0;False;0;False;-1;1519e3b659b5fc248abd68be06f3429e;1519e3b659b5fc248abd68be06f3429e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;132;-1334.563,-711.8544;Inherit;False;Property;_BaseTextureSaturation;Base Texture Saturation;2;0;Create;True;0;0;0;False;0;False;0;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;4;-1326.402,-554.3366;Inherit;False;Property;_BaseTextureTint;Base Texture Tint;1;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;136;-1028.7,-707.9258;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;117;-1390.874,124.3689;Inherit;True;Property;_TopTexture;Top Texture;3;0;Create;True;0;0;0;False;0;False;-1;a36684cfc68dfce44af56f21e48ee77f;a36684cfc68dfce44af56f21e48ee77f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;115;-1398.484,521.9647;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-1014.091,-393.6594;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;128;-1305.033,-53.33077;Inherit;False;Property;_TopTextureTint;Top Texture Tint;4;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DesaturateOpNode;131;-707.7758,-390.9871;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-995.1082,26.77634;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;118;-1222.483,521.9647;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;62;-448.394,2.672567;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-478.8745,289.9019;Float;False;Property;_Smoothness;Smoothness;8;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;6;;0;0;Standard;TidalFlask/Top Projection Standard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;109;0;91;0
WireConnection;96;0;124;2
WireConnection;96;1;71;0
WireConnection;113;0;109;0
WireConnection;113;1;109;2
WireConnection;134;0;113;0
WireConnection;134;1;135;0
WireConnection;108;0;96;0
WireConnection;136;0;132;0
WireConnection;117;1;134;0
WireConnection;115;0;108;0
WireConnection;115;1;72;0
WireConnection;2;0;4;0
WireConnection;2;1;1;0
WireConnection;131;0;2;0
WireConnection;131;1;136;0
WireConnection;129;0;128;0
WireConnection;129;1;117;0
WireConnection;118;0;115;0
WireConnection;62;0;131;0
WireConnection;62;1;129;0
WireConnection;62;2;118;0
WireConnection;0;0;62;0
WireConnection;0;4;61;0
ASEEND*/
//CHKSM=69CF6309CAD340AFB9600163894F83D5B3051A41