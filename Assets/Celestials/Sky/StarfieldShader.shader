Shader "Custom/Starfield" {
    Properties {
        _Color ("Main Color", Color) = (1,1,1,1)
    }
    SubShader {
        Pass {
            Tags {"QUEUE"="2000" "RenderType"="Background"}
            Cull Off
            ZWrite Off
			ZTest Always
			Lighting Off

			BindChannels {
				Bind "Color", color
				Bind "Vertex", vertex
				Bind "TexCoord", texcoord
			}

			Blend SrcAlpha OneMinusSrcAlpha

            SetTexture [_] {
                ConstantColor [_Color]
                Combine constant * primary
            }
        }
    }
}