// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Divine_Words/RMA_Complex"
{
	Properties
	{
		_AlbedoMap("Albedo Map", 2D) = "white" {}
		[NoScaleOffset]_RMAMap("RMA Map", 2D) = "white" {}
		[NoScaleOffset][Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset][Normal]_MainTex1("Detail Normal", 2D) = "white" {}
		_DetailNormalUVTiling("Detail Normal UV Tiling", Vector) = (1,1,0,0)
		_Color("Color", Color) = (1,1,1,1)
		_AOColor("AO Color", Color) = (1,1,1,1)
		_Rough("Roughness", Range( 0 , 4)) = 1
		_BumpScale("Normal Intensity", Range( 0 , 2)) = 1
		_BumpScale1("Detail Normal Intensity", Range( 0 , 2)) = 1
		_TriplanarHardness("Triplanar Hardness", Range( 0 , 1)) = 0
		_BlendHeight("Blend Height", Range( -2 , 5)) = 0
		_BlendContrast("Blend Contrast", Range( 0 , 2)) = 0
		_GroundAlbedo("Ground Albedo", 2D) = "white" {}
		_DarkenColor("Darken Color", Color) = (1,1,1,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _NormalMap;
		uniform sampler2D _AlbedoMap;
		uniform float4 _AlbedoMap_ST;
		uniform float _BumpScale;
		uniform sampler2D _MainTex1;
		uniform float4 _DetailNormalUVTiling;
		uniform float _TriplanarHardness;
		uniform float _BumpScale1;
		uniform float4 _DarkenColor;
		uniform sampler2D _GroundAlbedo;
		uniform float4 _GroundAlbedo_ST;
		uniform float4 _AOColor;
		uniform float4 _Color;
		uniform sampler2D _RMAMap;
		uniform float _BlendHeight;
		uniform float _BlendContrast;
		uniform float _Rough;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_AlbedoMap = i.uv_texcoord * _AlbedoMap_ST.xy + _AlbedoMap_ST.zw;
			float4 break39 = _DetailNormalUVTiling;
			float2 appendResult43 = (float2(break39.x , break39.y));
			float3 ase_worldPos = i.worldPos;
			float3 break40 = ase_worldPos;
			float2 appendResult45 = (float2(break40.y , break40.z));
			float2 appendResult46 = (float2(break39.z , break39.w));
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 temp_cast_1 = ((5.0 + (_TriplanarHardness - 0.0) * (100.0 - 5.0) / (1.0 - 0.0))).xxx;
			float3 temp_output_38_0 = pow( abs( ase_worldNormal ) , temp_cast_1 );
			float3 break42 = temp_output_38_0;
			float3 break59 = ( temp_output_38_0 / ( break42.x + break42.y + break42.z ) );
			float2 appendResult44 = (float2(break40.x , break40.z));
			float2 appendResult41 = (float2(break40.x , break40.y));
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, uv0_AlbedoMap ), _BumpScale ) , UnpackScaleNormal( ( float4( 0,0,0,0 ) + float4( 0,0,0,0 ) + ( tex2D( _MainTex1, ( ( appendResult43 * appendResult45 ) + appendResult46 ) ) * break59.x ) + ( tex2D( _MainTex1, ( ( appendResult43 * appendResult44 ) + appendResult46 ) ) * break59.y ) + ( tex2D( _MainTex1, ( ( appendResult41 * appendResult43 ) + appendResult46 ) ) * break59.z ) ), _BumpScale1 ) );
			float2 uv_GroundAlbedo = i.uv_texcoord * _GroundAlbedo_ST.xy + _GroundAlbedo_ST.zw;
			float4 blendOpSrc30 = _AOColor;
			float4 blendOpDest30 = ( _Color * tex2D( _AlbedoMap, uv0_AlbedoMap ) );
			float4 tex2DNode2 = tex2D( _RMAMap, uv0_AlbedoMap );
			float4 lerpBlendMode30 = lerp(blendOpDest30,( blendOpSrc30 * blendOpDest30 ),( 1.0 - tex2DNode2.b ));
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float smoothstepResult160 = smoothstep( _BlendHeight , ( _BlendHeight + _BlendContrast ) , (mul( unity_ObjectToWorld, float4( ase_vertex3Pos , 0.0 ) ).xyz).y);
			float4 lerpResult107 = lerp( ( _DarkenColor * tex2D( _GroundAlbedo, uv_GroundAlbedo ) ) , ( saturate( lerpBlendMode30 )) , smoothstepResult160);
			o.Albedo = lerpResult107.rgb;
			o.Metallic = tex2DNode2.g;
			o.Smoothness = ( 1.0 - ( _Rough * tex2DNode2.r ) );
			o.Occlusion = tex2DNode2.b;
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
			#pragma target 3.0
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
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
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
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
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
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
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
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
-76.30475;-157.4574;1240;728;5194.458;2040.155;6.41826;True;False
Node;AmplifyShaderEditor.WorldNormalVector;33;-3198.169,1869.245;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;32;-3364,2042.169;Inherit;False;Property;_TriplanarHardness;Triplanar Hardness;10;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;36;-3002.136,2051.104;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;4;FLOAT;100;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;34;-3494.478,1518.259;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector4Node;35;-3492.186,1007.066;Inherit;False;Property;_DetailNormalUVTiling;Detail Normal UV Tiling;4;0;Create;True;0;0;False;0;1,1,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;37;-2958.018,1871.787;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;39;-3232.381,1006.929;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.PowerNode;38;-2770.785,1867.323;Inherit;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;40;-3235.36,1518.783;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;41;-2856.055,1402.654;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;45;-2855.916,1659.491;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;42;-2564.081,1998.755;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;43;-2848.553,1009.358;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-2857.363,1532.366;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2559.897,1546.068;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-2561.844,1418.775;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-2846.335,1138.421;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-2555.786,1669.235;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-2264.105,1998.988;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;51;-2263.695,1167.427;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;53;-2261.926,1287.881;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;54;-2377.136,898.768;Inherit;True;Property;_MainTex1;Detail Normal;3;2;[NoScaleOffset];[Normal];Create;False;0;0;False;0;49dee0e66e3020c4ca867d088b8c50ba;49dee0e66e3020c4ca867d088b8c50ba;True;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;55;-2109.118,1873.85;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;52;-2261.287,1406.817;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;13;-1909.39,5.718911;Float;True;Property;_RMAMap;RMA Map;1;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;56;-1949.714,1509.97;Inherit;True;Property;_TextureSample3;Texture Sample 3;3;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1909.25,-245.6892;Float;True;Property;_AlbedoMap;Albedo Map;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-1915.031,-376.9936;Inherit;False;0;12;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;59;-1937.608,1870.493;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;57;-1952.997,1251.956;Inherit;True;Property;_TextureSample4;Texture Sample 4;2;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ObjectToWorldMatrixNode;163;-1836.536,-1588.581;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.PosVertexDataNode;156;-1787.739,-1481.145;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;58;-1956.505,995.6201;Inherit;True;Property;_TextureSample5;Texture Sample 5;4;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;114;-1628.275,-1086.769;Inherit;False;Property;_BlendContrast;Blend Contrast;12;0;Create;True;0;0;False;0;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1442.95,1524.649;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;1;-1401.945,-251.2451;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;158;-1549.807,-1517.815;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1664.205,-1232.246;Inherit;False;Property;_BlendHeight;Blend Height;11;0;Create;True;0;0;False;0;0;0;-2;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-1400.679,4.706662;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;14;-1904.766,264.9386;Float;True;Property;_NormalMap;Normal Map;2;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;True;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1437.786,1261.801;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;-1447.387,1008.756;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;15;-1264.569,-500.2498;Float;False;Property;_Color;Color;5;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;28;-1008.354,-247.033;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-1011.804,-375.7439;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;121;-984.0931,-1841.569;Inherit;False;Property;_DarkenColor;Darken Color;14;0;Create;True;0;0;False;0;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-1022.62,-96.94854;Float;False;Property;_Rough;Roughness;7;0;Create;False;0;0;False;0;1;2;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-1002.956,-632.4473;Float;False;Property;_AOColor;AO Color;6;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;63;-1116.576,1190.878;Inherit;False;5;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;3;-1399.293,264.2914;Inherit;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;118;-1037.013,-1618.692;Inherit;True;Property;_GroundAlbedo;Ground Albedo;13;0;Create;True;0;0;False;0;-1;c6f91c43fb3eef44aa609861cc403736;c6f91c43fb3eef44aa609861cc403736;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;66;-1119.229,1446.303;Float;False;Property;_BumpScale1;Detail Normal Intensity;9;0;Create;False;0;0;False;0;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;159;-1349.791,-1526.591;Inherit;False;False;True;False;True;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;-1201.191,-1185.987;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1074.17,376.4446;Float;False;Property;_BumpScale;Normal Intensity;8;0;Create;False;0;0;False;0;1;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;30;-749.5782,-380.8176;Inherit;False;Multiply;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;160;-1093.06,-1374.195;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;65;-751.3641,1193.915;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.UnpackScaleNormalNode;22;-755.8606,263.4785;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-688.9709,-44.96913;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-632.4066,-1832.973;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;31;-471.1286,-43.65039;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;67;-355.032,482.3547;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;107;-471.6815,-1430.6;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;473.05,-48.57727;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Divine_Words/RMA_Complex;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;103;-2026.106,-751.2535;Inherit;False;1921.996;1417.177;;0;Base Parameters;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;164;-1886.536,-1891.569;Inherit;False;1603.255;919.4001;;0;Blend;0,0,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;104;-3544.477,848.768;Inherit;False;3057.114;1404.334;;0;Triplanar;0,0,0,1;0;0
WireConnection;36;0;32;0
WireConnection;37;0;33;0
WireConnection;39;0;35;0
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;40;0;34;0
WireConnection;41;0;40;0
WireConnection;41;1;40;1
WireConnection;45;0;40;1
WireConnection;45;1;40;2
WireConnection;42;0;38;0
WireConnection;43;0;39;0
WireConnection;43;1;39;1
WireConnection;44;0;40;0
WireConnection;44;1;40;2
WireConnection;50;0;43;0
WireConnection;50;1;45;0
WireConnection;47;0;41;0
WireConnection;47;1;43;0
WireConnection;46;0;39;2
WireConnection;46;1;39;3
WireConnection;49;0;43;0
WireConnection;49;1;44;0
WireConnection;48;0;42;0
WireConnection;48;1;42;1
WireConnection;48;2;42;2
WireConnection;51;0;50;0
WireConnection;51;1;46;0
WireConnection;53;0;49;0
WireConnection;53;1;46;0
WireConnection;55;0;38;0
WireConnection;55;1;48;0
WireConnection;52;0;47;0
WireConnection;52;1;46;0
WireConnection;56;0;54;0
WireConnection;56;1;52;0
WireConnection;59;0;55;0
WireConnection;57;0;54;0
WireConnection;57;1;53;0
WireConnection;58;0;54;0
WireConnection;58;1;51;0
WireConnection;62;0;56;0
WireConnection;62;1;59;2
WireConnection;1;0;12;0
WireConnection;1;1;6;0
WireConnection;158;0;163;0
WireConnection;158;1;156;0
WireConnection;2;0;13;0
WireConnection;2;1;6;0
WireConnection;60;0;57;0
WireConnection;60;1;59;1
WireConnection;61;0;58;0
WireConnection;61;1;59;0
WireConnection;28;0;2;3
WireConnection;19;0;15;0
WireConnection;19;1;1;0
WireConnection;63;2;61;0
WireConnection;63;3;60;0
WireConnection;63;4;62;0
WireConnection;3;0;14;0
WireConnection;3;1;6;0
WireConnection;159;0;158;0
WireConnection;161;0;113;0
WireConnection;161;1;114;0
WireConnection;30;0;16;0
WireConnection;30;1;19;0
WireConnection;30;2;28;0
WireConnection;160;0;159;0
WireConnection;160;1;113;0
WireConnection;160;2;161;0
WireConnection;65;0;63;0
WireConnection;65;1;66;0
WireConnection;22;0;3;0
WireConnection;22;1;18;0
WireConnection;27;0;17;0
WireConnection;27;1;2;1
WireConnection;124;0;121;0
WireConnection;124;1;118;0
WireConnection;31;0;27;0
WireConnection;67;0;22;0
WireConnection;67;1;65;0
WireConnection;107;0;124;0
WireConnection;107;1;30;0
WireConnection;107;2;160;0
WireConnection;0;0;107;0
WireConnection;0;1;67;0
WireConnection;0;3;2;2
WireConnection;0;4;31;0
WireConnection;0;5;2;3
ASEEND*/
//CHKSM=9776D0DD2CAA83E38F0AB71AEB074EC17056607F