Shader "Custom/MyFirstShader"
{
    Properties
    {
        //_BaseColor : 셰이더에서 사용하는 변수와 동일
        //Base Color : 인스펙터창에서 보여지는 이름
        //Color : 타입, Color 타입은 RGBA값을 가진다
        //(0,0,0,0) : 초기값, 0~1사이의 값으로 설정
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap("Base Map", 2D) = "white" {}

        //_MyColor("My Color", Color) = (1, 1, 1, 1)
        //_MyVector("My Vector", Vector) = (1, 1, 1, 1)
        //_MyFloat("My Float", Float) = 0.5
        //_MyTexture("My Texture", 2D) = "white" {}
        //_MyCubeMap("My CubeMap", Cube) = "" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // HLSLPROGRAM과 ENDHLSL 사이에 작성된 코드는 HLSL 코드로 인식된다
            HLSLPROGRAM

            // vert : vertex 함수 이름
            #pragma vertex vert
            // frag : fragment 함수 이름
            #pragma fragment frag

            //HLSL 라이브러리 파일을 포함시킨다
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            //버텍스 정보중에 가져올 데이터 정의(그리기에 필요한 데이터)
            //vert 함수의 매개변수로 전달된다
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            //버텍스 셰이더에서 프래그먼트 셰이더로 전달할 데이터 정의
            //vert 함수의 연산의 결과로 반환값으로 전달된다
            //frag 함수의 매개변수로 전달된다
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END
            
            // vertex 함수 정의
            // vert 함수는 Attributes 구조체를 매개변수로 받아서 Varyings 구조체를 반환한다
            // 반환하는 Varyings 구조체는 프래그먼트 frag 함수의 매개변수로 전달된다
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            // fragment 함수 정의
            // frag 함수는 Varyings 구조체를 매개변수로 받아서 half4를 반환한다
            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return color;
            }
            ENDHLSL
        }
    }
}
