 Shader "Projector/Cone Projector" {
 Properties 
     {
         _Color ("Main Color", Color) = (1,1,1,1)       
         _Cutoff ("Alpha Cutoff" , Range(0, 1)) = .5
         _MainTex ("Texture", 2D) = ""
     }
 
 SubShader
     {
         ZWrite off
         Fog { Color (0, 0, 0) }
         Alphatest Greater [_Cutoff] 
         Cull Off
         Color [_Color]
         ColorMask RGB
         Blend OneMinusSrcAlpha SrcAlpha
         Offset -1, -1
         
         Pass
         {
             SetTexture[_MainTex] { 
                 combine texture * primary, ONE - texture
                    Matrix [_Projector]
             }
         }
 
     }
 }