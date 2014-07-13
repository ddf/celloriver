// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

Shader "Marmoset/Mobile/Bumped Diffuse IBL" {
	Properties {
		_Color   ("Diffuse Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse(RGB) Alpha(A)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) 	= "bump" {}
		//slots for custom lighting cubemaps
		_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
		_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}
	}
	
	SubShader {
		Tags {
			"Queue"="Geometry"
			"RenderType"="Opaque"
		}
		LOD 350
		//diffuse LOD 200
		//diffuse-spec LOD 250
		//bumped-diffuse, spec 350
		//bumped-spec 400
		
		//mac stuff
		CGPROGRAM
		#ifdef SHADER_API_OPENGL	
			#pragma glsl
			#pragma glsl_no_auto_normalization 
		#endif
		#pragma target 3.0
		#pragma surface MarmosetSurf MarmosetDirect exclude_path:prepass noforwardadd
		#define MARMO_GAMMA
		
		#define MARMO_HQ
		#define MARMO_SKY_ROTATION
		#define MARMO_DIFFUSE_IBL
		//#define MARMO_SPECULAR_IBL
		#define MARMO_DIFFUSE_DIRECT
		//#define MARMO_SPECULAR_DIRECT
		#define MARMO_NORMALMAP
		//#define MARMO_MIP_GLOSS
		//#define MARMO_GLOW
		//#define MARMO_PREMULT_ALPHA
		
		#include "../MarmosetMobile.cginc"
		#include "../MarmosetCore.cginc"
		#include "../MarmosetInput.cginc"
		#include "../MarmosetDirect.cginc"
		#include "../MarmosetSurf.cginc"

		ENDCG
	}
	
	FallBack "Mobile/Bumped Diffuse"
}