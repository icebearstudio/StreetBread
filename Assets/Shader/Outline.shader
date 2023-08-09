// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Outlined/Silhouette Only" {
    Properties {
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (0.0, 50)) = .05
    }
    
    CGINCLUDE
    #include "UnityCG.cginc"
    
    struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
    };
    
    struct v2f {
        float4 pos : POSITION;
        float4 color : COLOR;
    };
    
    uniform float _Outline;
    uniform float4 _OutlineColor;
    
    v2f vert(appdata v) {
        v2f o;
        o.pos = v.vertex;
        o.pos.xyz += v.normal.xyz *_Outline*0.01;
        o.pos = UnityObjectToClipPos(o.pos);
        
        o.color = _OutlineColor;
        return o;
    }
    ENDCG
    
    SubShader {
        Tags { "Queue" = "Transparent" }
        
        Pass {
            Name "BASE"
            Cull Back
            Blend Zero One
            
            // uncomment this to hide inner details:
            //Offset -8, -8
            
            SetTexture [_OutlineColor] {
                ConstantColor (0,0,0,0)
                Combine constant
            }
        }
        
        // note that a vertex shader is specified here but its using the one above
        Pass {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            
            // you can choose what kind of blending mode you want for the outline
            //Blend SrcAlpha OneMinusSrcAlpha // Normal
            //Blend One One // Additive
            Blend One OneMinusDstColor // Soft Additive
            //Blend DstColor Zero // Multiplicative
            //Blend DstColor SrcColor // 2x Multiplicative
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            half4 frag(v2f i) :COLOR {
                return i.color;
            }
            ENDCG
        }
        
        
    }
    
    Fallback "Diffuse"
}