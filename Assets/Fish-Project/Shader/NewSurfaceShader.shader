// This shader is used to cut out only the shadows onto a transparent surface.A ground plane in my case.
// I later realized that ARFoundation comes with its own custom cutout shader already integrated
// Nevertheless, it was really interesting (but hard) to understand how this can be achieved.
// Ive seen many modifications of cutout shaders online and they all follow pretty much what is below.

Shader "UnlitShadows/UnlitShadowReceive" {

    //This shader is finding the white color or whatever object it is on and cutting it to a range of 0 (white) to 1 (transparent)
    //The resutl is only the shadow 
    Properties{ 
        _Color("Main Color", Color) = (1,1,1,1) 
        _MainTex("Base (RGB)", 2D) = "white" {} 
        _Cutoff("Cutout", Range(0,1)) = 0.5 
    }

    SubShader{ 
    
        Pass{ 
            Alphatest Greater[_Cutoff] SetTexture[_MainTex] //Set up the alpha test to only render pixels whose alpha value is within a certain range.

        } 

        Pass{ 
            Blend DstColor Zero Tags{
                 "LightMode" = "ForwardBase"
              }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #include "AutoLight.cginc"

            struct v2f {
                float4 pos : SV_POSITION; LIGHTING_COORDS(0,1) //Add some definitions to the structure that is passed from vertex to fragment program: add LIGHTING_COORDS macro before any texture interpolators. 
            };

            v2f vert(appdata_base v) { //Position, normal and one texture coordinate
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex); 
                TRANSFER_VERTEX_TO_FRAGMENT(o); //o is the variable that holds the output structure.
                return o; 
            }

            fixed4 frag(v2f i) : COLOR{
                float attenuation = LIGHT_ATTENUATION(i); //The light attenuation macro returns a single float value that combines light attenuation, cookie and shadow - the "illuminance".
                return attenuation;

            }
             
            ENDCG 
            
         } 
     } 
     
   Fallback "Transparent/Cutout/VertexLit"
   
 } 