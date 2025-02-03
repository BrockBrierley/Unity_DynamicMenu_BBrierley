// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VIVID Arts/Lit"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (1,1,1,0)
		_BaseColorMap("Base Map", 2D) = "white" {}
		_MaskMap("Mask Map", 2D) = "white" {}
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Smoothness("Smoothness", Range( 0 , 1)) = 1
		_Occlusion("Occlusion", Range( 0 , 1)) = 1
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( 0 , 2)) = 1
		_DetailMap("Detail Map", 2D) = "gray" {}
		_DetailAlbedoScale("Detail Albedo Scale", Range( 0 , 2)) = 1
		_DetailNormalScale("Detail Normal Scale", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform sampler2D _DetailMap;
		uniform float4 _DetailMap_ST;
		uniform float _DetailNormalScale;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float4 _BaseColor;
		uniform sampler2D _BaseColorMap;
		uniform float4 _BaseColorMap_ST;
		uniform float _DetailAlbedoScale;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Occlusion;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float2 uv_DetailMap = i.uv_texcoord * _DetailMap_ST.xy + _DetailMap_ST.zw;
			float4 tex2DNode14 = tex2D( _DetailMap, uv_DetailMap );
			float4 appendResult22 = (float4(tex2DNode14.a , tex2DNode14.g , 0.0 , 1.0));
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode12 = tex2D( _MaskMap, uv_MaskMap );
			float3 lerpResult19 = lerp( float3( 0,0,1 ) , UnpackScaleNormal( appendResult22, _DetailNormalScale ) , tex2DNode12.b);
			o.Normal = BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) , lerpResult19 );
			float2 uv_BaseColorMap = i.uv_texcoord * _BaseColorMap_ST.xy + _BaseColorMap_ST.zw;
			float3 temp_output_26_0 = ( _BaseColor.rgb * tex2D( _BaseColorMap, uv_BaseColorMap ).rgb );
			float lerpResult21 = lerp( 0.5 , tex2DNode14.r , _DetailAlbedoScale);
			float3 lerpResult20 = lerp( temp_output_26_0 , ( temp_output_26_0 * ( lerpResult21 * 2.0 ) ) , tex2DNode12.b);
			o.Albedo = lerpResult20;
			o.Metallic = ( tex2DNode12.r * _Metallic );
			o.Smoothness = ( tex2DNode12.a * _Smoothness );
			float lerpResult17 = lerp( 1.0 , tex2DNode12.g , _Occlusion);
			o.Occlusion = lerpResult17;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.TexturePropertyNode;10;-3296,640;Inherit;True;Property;_DetailMap;Detail Map;8;0;Create;True;0;0;0;False;0;False;None;None;False;gray;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;14;-2976,704;Inherit;True;Property;_TextureSample3;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.RangedFloatNode;6;-2976,592;Inherit;False;Property;_DetailAlbedoScale;Detail Albedo Scale;9;0;Create;True;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-2272,-336;Inherit;True;Property;_BaseColorMap;Base Map;1;0;Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.LerpOp;21;-2592,672;Inherit;False;3;0;FLOAT;0.5;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-2704,992;Inherit;False;Property;_DetailNormalScale;Detail Normal Scale;10;0;Create;True;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;8;-2832,1200;Inherit;True;Property;_MaskMap;Mask Map;2;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;11;-1952,-320;Inherit;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.ColorNode;2;-1888,-576;Inherit;False;Property;_BaseColor;Base Color;0;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2592,816;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;9;-2400,256;Inherit;True;Property;_NormalMap;Normal Map;6;0;Create;True;0;0;0;False;0;False;None;None;False;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;5;-2448,496;Inherit;False;Property;_NormalScale;Normal Scale;7;0;Create;True;0;0;0;False;0;False;1;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-2512,1216;Inherit;True;Property;_TextureSample1;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-1520,-448;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-2240,656;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;24;-2352,848;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;18;-2032,1360;Inherit;False;Property;_Occlusion;Occlusion;5;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-2064,1504;Inherit;False;Property;_Smoothness;Smoothness;4;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-2096,1216;Inherit;False;Property;_Metallic;Metallic;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;19;-1984,864;Inherit;False;3;0;FLOAT3;0,0,1;False;1;FLOAT3;0,0,0;False;2;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;13;-2064,336;Inherit;True;Property;_TextureSample2;Texture Sample 0;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-1280,-320;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1728,1136;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-1712,1424;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;17;-1712,1264;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;25;-1664,432;Inherit;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;20;-1024,-416;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-16,-144;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VIVID Arts/Lit;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;17;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;14;0;10;0
WireConnection;21;1;14;1
WireConnection;21;2;6;0
WireConnection;11;0;1;0
WireConnection;22;0;14;4
WireConnection;22;1;14;2
WireConnection;12;0;8;0
WireConnection;26;0;2;5
WireConnection;26;1;11;5
WireConnection;23;0;21;0
WireConnection;24;0;22;0
WireConnection;24;1;7;0
WireConnection;19;1;24;0
WireConnection;19;2;12;3
WireConnection;13;0;9;0
WireConnection;13;5;5;0
WireConnection;27;0;26;0
WireConnection;27;1;23;0
WireConnection;15;0;12;1
WireConnection;15;1;3;0
WireConnection;16;0;12;4
WireConnection;16;1;4;0
WireConnection;17;1;12;2
WireConnection;17;2;18;0
WireConnection;25;0;13;0
WireConnection;25;1;19;0
WireConnection;20;0;26;0
WireConnection;20;1;27;0
WireConnection;20;2;12;3
WireConnection;0;0;20;0
WireConnection;0;1;25;0
WireConnection;0;3;15;0
WireConnection;0;4;16;0
WireConnection;0;5;17;0
ASEEND*/
//CHKSM=D6618B59645D173C2C88EDFDA28BA75245A06D40