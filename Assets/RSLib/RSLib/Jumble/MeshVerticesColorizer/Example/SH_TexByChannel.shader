// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VertexColor/Blend"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_NormalMain("NormalMain", 2D) = "bump" {}
		[NoScaleOffset]_MraMain("MraMain", 2D) = "white" {}
		_SecondaryTex("SecondaryTex", 2D) = "white" {}
		[NoScaleOffset][Normal]_NormalSecondary("Normal Secondary", 2D) = "bump" {}
		[NoScaleOffset]_MraSecondary("MraSecondary", 2D) = "white" {}
		_Hardness("Hardness", Range( 0.001 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _NormalMain;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform sampler2D _NormalSecondary;
		uniform sampler2D _SecondaryTex;
		uniform float4 _SecondaryTex_ST;
		uniform float _Hardness;
		uniform sampler2D _MraMain;
		uniform sampler2D _MraSecondary;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 uv0_SecondaryTex = i.uv_texcoord * _SecondaryTex_ST.xy + _SecondaryTex_ST.zw;
			float temp_output_29_0 = saturate( ( i.vertexColor.r / _Hardness ) );
			float3 lerpResult17 = lerp( UnpackNormal( tex2D( _NormalMain, uv0_MainTex ) ) , UnpackNormal( tex2D( _NormalSecondary, uv0_SecondaryTex ) ) , temp_output_29_0);
			o.Normal = lerpResult17;
			float4 lerpResult8 = lerp( tex2D( _MainTex, uv0_MainTex ) , tex2D( _SecondaryTex, uv0_SecondaryTex ) , temp_output_29_0);
			o.Albedo = lerpResult8.rgb;
			float4 lerpResult24 = lerp( tex2D( _MraMain, uv0_MainTex ) , tex2D( _MraSecondary, uv0_SecondaryTex ) , temp_output_29_0);
			float4 break25 = lerpResult24;
			o.Metallic = break25;
			o.Smoothness = break25.g;
			o.Occlusion = break25.b;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17700
1034;73;1141;906;2218.833;-2213.544;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;27;-1561.552,2691.618;Inherit;False;Property;_Hardness;Hardness;6;0;Create;True;0;0;False;0;1;0.001;0.001;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;1;-1470.331,2500.427;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;28;-1278.096,2623.736;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;20;-1376.369,1728.591;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;19;-1378.117,1908.051;Inherit;True;Property;_MraSecondary;MraSecondary;5;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;18;-1372.468,1499.791;Inherit;True;Property;_MraMain;MraMain;2;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-1371.306,2101.335;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;22;-1091.669,1646.691;Inherit;True;Property;_TextureSample4;Texture Sample 4;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;12;-1409.18,590.6987;Inherit;True;Property;_NormalSecondary;Normal Secondary;4;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;False;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SaturateNode;29;-1149.266,2625.415;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-1381.084,-224.8298;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-1392.974,431.9453;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;11;-1386.112,203.1455;Inherit;True;Property;_NormalMain;NormalMain;1;2;[NoScaleOffset];[Normal];Create;True;0;0;False;0;None;None;False;bump;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1413.682,784.4283;Inherit;False;0;5;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1365.147,-672.5737;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;23;-1107.069,1941.557;Inherit;True;Property;_TextureSample5;Texture Sample 5;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;3;-1361.246,-901.3735;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.TexturePropertyNode;5;-1390.895,-459.1143;Inherit;True;Property;_SecondaryTex;SecondaryTex;3;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.LerpOp;24;-693.9603,1829.433;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;2;-1080.447,-754.4738;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;-1114.913,297.0366;Inherit;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-1072,-336;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1111.76,680.121;Inherit;True;Property;_TextureSample3;Texture Sample 3;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;25;-410.4679,1829.807;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;17;-349.1917,499.4926;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;8;-717.2693,-514.2836;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;490.3484,432.6422;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Cour/04_VertecColor/2_VCBlending;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;28;0;1;1
WireConnection;28;1;27;0
WireConnection;22;0;18;0
WireConnection;22;1;20;0
WireConnection;29;0;28;0
WireConnection;23;0;19;0
WireConnection;23;1;21;0
WireConnection;24;0;22;0
WireConnection;24;1;23;0
WireConnection;24;2;29;0
WireConnection;2;0;3;0
WireConnection;2;1;4;0
WireConnection;15;0;11;0
WireConnection;15;1;13;0
WireConnection;6;0;5;0
WireConnection;6;1;7;0
WireConnection;16;0;12;0
WireConnection;16;1;14;0
WireConnection;25;0;24;0
WireConnection;17;0;15;0
WireConnection;17;1;16;0
WireConnection;17;2;29;0
WireConnection;8;0;2;0
WireConnection;8;1;6;0
WireConnection;8;2;29;0
WireConnection;0;0;8;0
WireConnection;0;1;17;0
WireConnection;0;3;25;0
WireConnection;0;4;25;1
WireConnection;0;5;25;2
ASEEND*/
//CHKSM=A7B3907C065B17370DE35420161AA936593A7260