Shader "Custom/WorldCoord Diffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_Emissive ("Emissive (RGB)", 2D) = "white" {}
	_BaseScale ("Base Tiling", Vector) = (1,1,1,0)
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 150

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
sampler2D _Emissive;

fixed4 _Color;
fixed3 _BaseScale;

struct Input {
	float2 uv_MainTex;
	float3 worldPos;
	float3 worldNormal;

};

void surf (Input IN, inout SurfaceOutput o) 
{
	
	fixed4 tex2XY = tex2D(_Emissive, IN.worldPos.xy * _BaseScale.z);// IN.uv_MainTex);
	fixed4 tex2XZ = tex2D(_Emissive, IN.worldPos.xz * _BaseScale.y);// IN.uv_MainTex);
	fixed4 tex2YZ = tex2D(_Emissive, IN.worldPos.yz * _BaseScale.x);// IN.uv_MainTex);

	fixed3 mask = fixed3(
		dot (IN.worldNormal, fixed3(0,0,1)),
		dot (IN.worldNormal, fixed3(0,1,0)),
		dot (IN.worldNormal, fixed3(1,0,0)));
	
	
	fixed4 tex2 =
		tex2XY * abs(mask.x) +
		tex2XZ * abs(mask.y) +
		tex2YZ * abs(mask.z);
	

	o.Albedo = _Color;
	o.Emission = tex2 *2;
}
ENDCG
}

FallBack "Diffuse"
}
