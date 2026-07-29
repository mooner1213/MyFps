using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*
Lit Shader : 라이팅을 처리한 셰이터
UnLit Shader : 라이팅을 처리하지 않은 셰이더, 빛의 영향을 받지 않고, 항상 같은 색으로 출력되는 셰이더

 
Lighting 처리
Ambient : 주변광, 환경광, 빛이 물체에 반사하여 나온 반사광
전체적인 색감 - 광원의 위치와 무관하게 똑같은 양으로 모든 점에서 반사되는 색

Diffuse : 확산광, 물체 표면에 입사한 빛이 물체 표면에서 여러 방향으로 흩어져 나오는 빛
자신의 색, Main Color, Albedo 

Specular : 반사광, 물체 표면에 입사한 빛이 특정 방향으로 반사되어 나오는 빛

Emissive : 발광광, 물체 표면에서 스스로 빛을 내는 빛
다른 메시에는 영향을 주지 않고, 자신만 빛을 내는 색

<Shader>
화면에 출력할 픽셀의 위치(Vertex)와 색상(Pragment)을 결정하는 프로그램

Shader 프로그램
ShaderLab : 유니티에서 사용하는 셰이더 스크립트 언어
언어는 HLSL, Cg, GLSL
HLSL : High Level Shader Language
CG : C for Graphics
GLSL : OpenGL Shading Language

ShaderLab에 작성하는 셰이더 프로그램은 크게 3가지로 나뉜다
Fixed Function : 고정함수 셰이더, 고정 기능 파이프라인, 유니티에서 제공하는 기본 셰이더
Surface Shader : 표면 셰이더, 유니티에서 제공하는 표면 셰이더, 고정함수 셰이더보다 더 많은 기능을 제공
Vertex and Fragment Shader : 버텍스와 프래그먼트 셰이더, 유니티에서 제공하는 표면 셰이더보다 더 많은 기능을 제공, 직접 작성해야 함


Shader "셰이더경로/셰이더이름"
{
    //인스펙터창에서 입력받을 수 있는 변수들을 정의하는 곳
    Properties
    {        
    }
    //고사양
    SubShader
    {
        Pass
        {
        }
        Pass
        {
        }
    }
    //중사양
    SubShader
    {
        
    }
    //저사양
    SubShader
    {
        
    }
    fallback "Diffuse"
}

*/