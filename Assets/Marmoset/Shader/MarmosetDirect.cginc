// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

#ifndef MARMOSET_DIRECT_CGINC
#define MARMOSET_DIRECT_CGINC

//deferred lighting
inline half4 LightingMarmosetDirect_PrePass( MarmosetOutput s, half4 light ) {
	half4 frag = half4(0.0,0.0,0.0,1.0);
	#ifdef MARMO_DIFFUSE_DIRECT
		frag.rgb = s.Albedo * light.rgb;
		frag.a = s.Alpha;
	#endif
	#ifdef MARMO_SPECULAR_DIRECT
		frag.rgb += light.rgb * light.a * s.SpecularRGB * 0.15; //*0.15 to match forward lighting
	#endif
	return frag;
}

//forward lighting
inline half4 LightingMarmosetDirect( MarmosetOutput s, half3 lightDir, half3 viewDir, half atten ) {
	half4 frag = half4(0.0,0.0,0.0,s.Alpha);
	
	#if defined(MARMO_DIFFUSE_DIRECT) || defined(MARMO_SPECULAR_DIRECT)
		half3 L = lightDir;
		half3 N = s.Normal;
		#ifdef MARMO_HQ
			L = normalize(L);
		#endif
	#endif
		
	#ifdef MARMO_DIFFUSE_DIRECT
		half dp = saturate(dot(N,L));
		half3 diff = (2.0 * dp) * s.Albedo.rgb; //*2.0 to match Unity
		frag.rgb = diff * _LightColor0.rgb;
	#endif
	
	#ifdef MARMO_SPECULAR_DIRECT
		half3 H = normalize(viewDir+L);
		float specRefl = saturate(dot(N,H));
		half3 spec = pow(specRefl, s.Specular*256.0);
		#ifdef MARMO_HQ
			//self-shadowing blinn
			#ifdef MARMO_DIFFUSE_DIRECT
				spec *= saturate(10.0*dp);
			#else
				spec *= saturate(10.0*dot(N,L));
			#endif
		#endif
		spec *= _LightColor0.rgb;
		frag.rgb += (0.5 * spec) * s.SpecularRGB; //*0.5 to match Unity
	#endif
	frag.rgb *= atten;
	return frag;
}

#endif