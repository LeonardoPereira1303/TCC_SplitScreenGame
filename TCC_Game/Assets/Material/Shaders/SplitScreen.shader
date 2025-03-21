Shader "Mask/SplitScreen" {
	//Simple depthmask shader 
	SubShader {
	    Tags {"Queue" = "Geometry+10"}
		ColorMask 0
		ZWrite On

	    Pass {}
	}
}
