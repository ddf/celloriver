// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

//#ifndef MARMOSET_INPUT_CGINC
//#define MARMOSET_INPUT_CGINC

sampler2D 	_MainTex;
half4 		_ExposureIBL;

#ifdef MARMO_SKY_ROTATION
float4x4	_SkyMatrix;
#endif

#ifdef MARMO_DIFFUSE_DIRECT
samplerCUBE _DiffCubeIBL;
float4		_Color;
#endif

#ifdef MARMO_SPECULAR_DIRECT
sampler2D	_SpecTex;
samplerCUBE _SpecCubeIBL;
//float4	_SpecColor; //defined by unity
float		_SpecInt;
float		_Shininess;
float		_Fresnel;
#endif

#ifdef MARMO_NORMALMAP
sampler2D 	_BumpMap;
#endif

#ifdef MARMO_GLOW
sampler2D	_Illum;
float4		_GlowColor;
float		_GlowStrength;
float		_EmissionLM;
#endif

struct Input {
	float2 uv_MainTex;
	float3 worldNormal; //internal, required for the WorldNormalVector macro
	#if defined(MARMO_SPECULAR_DIRECT) || defined(MARMO_SPECULAR_IBL)
		float3 viewDir;
	#endif
	#ifdef MARMO_SPECULAR_IBL
		float3 worldRefl; //internal, required for the WorldReflVector macro
	#endif
	INTERNAL_DATA
};

struct MarmosetOutput {
	half3 Albedo;	//diffuse map RGB
	half Alpha;		//diffuse map A
	half3 Normal;	//world-space normal
	half3 Emission;	//contains IBL contribution
	half Specular;	//specular exponent (required by Unity)
	#ifdef MARMO_SPECULAR_DIRECT
		half3 SpecularRGB;	//specular mask
	#endif
};
//#endif