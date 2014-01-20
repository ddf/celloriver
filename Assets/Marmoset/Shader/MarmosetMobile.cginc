// Marmoset Skyshop
// Copyright 2013 Marmoset LLC
// http://marmoset.co

//rules for mobile shader permutations				
//nothing fancy
#ifdef MARMO_HQ 
#undef MARMO_HQ
#endif

#ifdef MARMO_SKY_ROTATION 
#undef MARMO_SKY_ROTATION
#endif

//texCUBElod is broken in Unity's iOS as of Unity 4.1, you may have to disable it here
#ifdef MARMO_MIP_GLOSS
#undef MARMO_MIP_GLOSS
//#define MARMO_BIAS_GLOSS
#endif


