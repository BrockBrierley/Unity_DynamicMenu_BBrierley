// Made with Amplify Shader Editor v1.9.0.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VIVID Arts/Foliage"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_BaseMap("BaseMap", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_MaskMap("MaskMap", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_HealthyColor("Healthy Color", Color) = (1,1,1,0)
		_DryColor("Dry Color", Color) = (1,1,1,0)
		_VertexOcclusion("Vertex Occlusion", Range( 0 , 1)) = 1
		_WindNormal("Wind Normal", 2D) = "bump" {}
		_WindStrength("Wind Strength", Float) = 0.2
		_WindSpeed("Wind Speed", Float) = 0.1
		_WindScale("Wind Scale", Float) = 0.7
		_Wind2Strength("Wind2 Strength", Float) = -0.5
		_Wind2Speed("Wind2 Speed", Float) = 0.1
		_Wind2Scale("Wind2 Scale", Float) = 0.24
		_ThicknessMax("Thickness Max", Range( 0 , 1)) = 1
		_ThicknessMin("Thickness Min", Range( 0 , 1)) = 0
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustom keepalpha addshadow fullforwardshadows exclude_path:deferred dithercrossfade vertex:vertexDataFunc 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Translucency;
		};

		uniform sampler2D _WindNormal;
		uniform float _WindSpeed;
		uniform float _WindScale;
		uniform float _WindStrength;
		uniform float _Wind2Speed;
		uniform float _Wind2Scale;
		uniform float _Wind2Strength;
		uniform sampler2D _BumpMap;
		uniform float4 _BumpMap_ST;
		uniform sampler2D _BaseMap;
		uniform float4 _BaseMap_ST;
		uniform float4 _HealthyColor;
		uniform float4 _DryColor;
		uniform sampler2D _MaskMap;
		uniform float4 _MaskMap_ST;
		uniform float _Smoothness;
		uniform float _VertexOcclusion;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _ThicknessMin;
		uniform float _ThicknessMax;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float mulTime60 = _Time.y * _WindSpeed;
			float3 tex2DNode65 = UnpackScaleNormal( tex2Dlod( _WindNormal, float4( ( ( ase_worldPos + mulTime60 ) * _WindScale ).xy, 0, 0.0) ), _WindStrength );
			float mulTime95 = _Time.y * _Wind2Speed;
			float4 appendResult93 = (float4(tex2DNode65.r , tex2DNode65.g , UnpackScaleNormal( tex2Dlod( _WindNormal, float4( ( ( ase_worldPos + mulTime95 ) * _Wind2Scale ).xy, 0, 0.0) ), _Wind2Strength ).r , 0.0));
			float4 WindOutput66 = ( appendResult93 * v.color.r );
			v.vertex.xyz += WindOutput66.xyz;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 uv_BumpMap = i.uv_texcoord * _BumpMap_ST.xy + _BumpMap_ST.zw;
			float3 NormalOutput37 = UnpackNormal( tex2D( _BumpMap, uv_BumpMap ) );
			o.Normal = NormalOutput37;
			float2 uv_BaseMap = i.uv_texcoord * _BaseMap_ST.xy + _BaseMap_ST.zw;
			float4 tex2DNode8 = tex2D( _BaseMap, uv_BaseMap );
			float4 lerpResult128 = lerp( _HealthyColor , _DryColor , i.vertexColor.b);
			float2 uv_MaskMap = i.uv_texcoord * _MaskMap_ST.xy + _MaskMap_ST.zw;
			float4 tex2DNode18 = tex2D( _MaskMap, uv_MaskMap );
			float MaskMapBlue107 = tex2DNode18.b;
			float4 lerpResult130 = lerp( tex2DNode8 , ( lerpResult128 * tex2DNode8 ) , MaskMapBlue107);
			float4 AlbedoOutput39 = lerpResult130;
			o.Albedo = AlbedoOutput39.rgb;
			float MaskMapAlpha42 = tex2DNode18.a;
			float SmoothnessOutput11 = ( _Smoothness * MaskMapAlpha42 );
			o.Smoothness = SmoothnessOutput11;
			float MaskMapGreen43 = tex2DNode18.g;
			float temp_output_34_0 = saturate( ((1.0 + (_VertexOcclusion - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) + (i.vertexColor.g - 0.0) * (1.0 - (1.0 + (_VertexOcclusion - 0.0) * (0.0 - 1.0) / (1.0 - 0.0))) / (1.0 - 0.0)) );
			float OcclusionOutput32 = ( MaskMapGreen43 * temp_output_34_0 );
			o.Occlusion = OcclusionOutput32;
			float MaskMapRed56 = tex2DNode18.r;
			float Thickness138 = (_ThicknessMin + (MaskMapRed56 - 0.0) * (_ThicknessMax - _ThicknessMin) / (1.0 - 0.0));
			float3 temp_cast_1 = (Thickness138).xxx;
			o.Translucency = temp_cast_1;
			o.Alpha = 1;
			float AlphaOutput48 = tex2DNode8.a;
			clip( AlphaOutput48 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19002
496.6667;848;1249.333;511.0001;2339.429;-407.7852;1.565689;True;False
Node;AmplifyShaderEditor.RangedFloatNode;94;-3193.659,-1514.593;Inherit;False;Property;_Wind2Speed;Wind2 Speed;16;0;Create;True;0;0;0;False;0;False;0.1;0.14;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-3035.644,-2060.14;Inherit;False;Property;_WindSpeed;Wind Speed;13;0;Create;True;0;0;0;False;0;False;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;71;-3069.464,-2352.672;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;95;-3002.838,-1523.173;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;60;-2844.823,-2068.72;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;104;-3440.068,-1883.654;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;45;-1336.701,-40.52637;Inherit;False;976.4014;354.2498;;6;56;107;43;42;18;17;MaskMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;46;-1963.916,471.6796;Inherit;False;1572.377;550.2935;;11;108;32;28;41;34;33;29;27;109;124;150;Occlusion;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1910.303,805.4166;Inherit;False;Property;_VertexOcclusion;Vertex Occlusion;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;75;-2548.681,-2282.552;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;-2706.696,-1737.005;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;17;-1286.701,15.76452;Inherit;True;Property;_MaskMap;MaskMap;3;0;Create;True;0;0;0;False;0;False;None;ed30aebbc91be824f9d7849cf2b5a8fa;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RangedFloatNode;80;-2489.186,-1932.168;Inherit;False;Property;_WindScale;Wind Scale;14;0;Create;True;0;0;0;False;0;False;0.7;1.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-2647.201,-1409.446;Inherit;False;Property;_Wind2Scale;Wind2 Scale;17;0;Create;True;0;0;0;False;0;False;0.24;0.32;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-928.5922,-1133.135;Inherit;False;2558.906;706.313;;11;130;39;48;26;8;128;25;9;129;127;131;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;129;-527.7479,-871.5919;Inherit;False;Property;_DryColor;Dry Color;8;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.8866891,0.9150943,0.5237332,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-2199.124,-2095.297;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TexturePropertyNode;9;49.71174,-721.8726;Inherit;True;Property;_BaseMap;BaseMap;1;0;Create;True;0;0;0;False;0;False;None;459772acd829e2340a074e57bdbecd14;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;18;-990.2473,9.473747;Inherit;True;Property;_TextureSample2;Texture Sample 2;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;27;-1913.916,569.6082;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;150;-1583.201,727.1859;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-2361.181,-1574.006;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-2368.079,-1827.843;Inherit;False;Property;_WindStrength;Wind Strength;12;0;Create;True;0;0;0;False;0;False;0.2;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-2526.094,-1282.296;Inherit;False;Property;_Wind2Strength;Wind2 Strength;15;0;Create;True;0;0;0;False;0;False;-0.5;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;127;-273.2472,-754.8395;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;25;-538.1122,-1064.457;Inherit;False;Property;_HealthyColor;Healthy Color;7;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.5526196,0.6980392,0.3803921,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;64;-2245.905,-2469.673;Inherit;True;Property;_WindNormal;Wind Normal;11;0;Create;True;0;0;0;False;0;False;None;eec9eafca2bc9f748b8b1180b01f9e20;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-595.7698,81.61027;Inherit;False;MaskMapGreen;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;128;40.06787,-948.1722;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;47;-1035.583,-333.7729;Inherit;False;856.4821;271.8869;;4;10;12;11;44;Smoothness;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;42;-590.9157,232.8731;Inherit;False;MaskMapAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;98;-1916,-1528.695;Inherit;True;Property;_TextureSample4;Texture Sample 4;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;8;392.011,-827.9984;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;107;-595.0071,157.922;Inherit;False;MaskMapBlue;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;65;-1757.986,-2074.242;Inherit;True;Property;_TextureSample3;Texture Sample 3;9;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;52;-2090.902,-377.595;Inherit;False;998.2661;289;;3;19;20;37;Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-597.6456,-1.468536;Inherit;False;MaskMapRed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;147;-3250.369,-928.6924;Inherit;False;871.5637;361.3878;;5;137;135;140;139;138;Thickness;1,1,1,1;0;0
Node;AmplifyShaderEditor.TFHCRemapNode;33;-1324.723,616.5355;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;-1059.188,644.3717;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;41;-1078.266,540.8911;Inherit;False;43;MaskMapGreen;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;93;-1274.118,-2065.765;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-3199.142,-773.7375;Inherit;False;Property;_ThicknessMin;Thickness Min;20;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;19;-2040.902,-318.595;Inherit;True;Property;_BumpMap;BumpMap;4;0;Create;True;0;0;0;False;0;False;None;a11547c12d97fba439e89e3b7105c8f6;True;bump;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.VertexColorNode;76;-987.226,-1516.249;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;135;-3171.338,-878.6924;Inherit;False;56;MaskMapRed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;44;-896.0361,-176.8861;Inherit;False;42;MaskMapAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-985.5823,-280.3521;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;0;False;0;False;0.5;0.38;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-3200.369,-682.3046;Inherit;False;Property;_ThicknessMax;Thickness Max;19;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;849.1706,-1011.646;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;812.6565,-785.4101;Inherit;False;107;MaskMapBlue;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;20;-1750.905,-327.595;Inherit;True;Property;_TextureSample1;Texture Sample 1;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;139;-2866.106,-878.2598;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-603.3135,-1574.667;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;130;1131.234,-940.9543;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-640.0674,-271.7994;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-789.6179,568.2142;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;32;-605.5732,590.8499;Inherit;False;OcclusionOutput;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;66;-164.5676,-1564.234;Inherit;False;WindOutput;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-433.1007,-283.773;Inherit;False;SmoothnessOutput;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;1368.475,-923.0988;Inherit;False;AlbedoOutput;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;37;-1320.637,-300.6241;Inherit;False;NormalOutput;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;51;-2090.788,-888.853;Inherit;False;1073.025;381.1818;;7;36;35;23;53;54;57;126;Translucency;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;138;-2606.805,-872.9194;Inherit;False;Thickness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;735.9713,-696.8003;Inherit;False;AlphaOutput;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;44.68276,31.42601;Inherit;False;32;OcclusionOutput;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-1307.193,827.0471;Inherit;True;56;MaskMapRed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;124;-1094.239,814.2731;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;109;-892.7655,696.9905;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-235.9738,170.8522;Inherit;False;Property;_ShadowThreshold;Shadow Threshold;5;0;Create;True;0;0;0;False;0;False;0.5;0.25;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-1990.69,-841.9551;Inherit;False;56;MaskMapRed;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-1839.407,-596.7689;Inherit;False;32;OcclusionOutput;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;106;-232.843,62.81815;Inherit;False;Property;_AlphaClipThreshold;Alpha Clip Threshold;6;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;84.61075,298.7217;Inherit;False;23;SSSOutput;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.DiffusionProfileNode;122;-151.9642,328.982;Float;False;Property;_DiffusionProfile;Diffusion Profile;18;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;105;-3076.181,-1894.236;Inherit;True;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1411.521,-799.6868;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1245.765,-787.4637;Inherit;False;SSSOutput;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-1601.229,-2365.719;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-1638.483,-821.1013;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;126;-1590.65,-703.6949;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;-2030.565,-750.3353;Inherit;False;Property;_Translucency;Translucency;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;38;38.91803,-137.0443;Inherit;False;37;NormalOutput;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;15;40.81453,-55.79237;Inherit;False;11;SmoothnessOutput;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;49;82.66884,127.7134;Inherit;False;48;AlphaOutput;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;40;43.08567,-232.9004;Inherit;False;39;AlbedoOutput;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;80.17809,382.8411;Inherit;False;66;WindOutput;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;123;68.80459,211.6041;Inherit;False;138;Thickness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;149;361.5296,-18.22533;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VIVID Arts/Foliage;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;False;Opaque;;AlphaTest;ForwardOnly;18;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;0;21;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;95;0;94;0
WireConnection;60;0;70;0
WireConnection;75;0;71;0
WireConnection;75;1;60;0
WireConnection;100;0;104;0
WireConnection;100;1;95;0
WireConnection;79;0;75;0
WireConnection;79;1;80;0
WireConnection;18;0;17;0
WireConnection;150;0;29;0
WireConnection;99;0;100;0
WireConnection;99;1;96;0
WireConnection;43;0;18;2
WireConnection;128;0;25;0
WireConnection;128;1;129;0
WireConnection;128;2;127;3
WireConnection;42;0;18;4
WireConnection;98;0;64;0
WireConnection;98;1;99;0
WireConnection;98;5;97;0
WireConnection;8;0;9;0
WireConnection;107;0;18;3
WireConnection;65;0;64;0
WireConnection;65;1;79;0
WireConnection;65;5;78;0
WireConnection;56;0;18;1
WireConnection;33;0;27;2
WireConnection;33;3;150;0
WireConnection;34;0;33;0
WireConnection;93;0;65;1
WireConnection;93;1;65;2
WireConnection;93;2;98;1
WireConnection;26;0;128;0
WireConnection;26;1;8;0
WireConnection;20;0;19;0
WireConnection;139;0;135;0
WireConnection;139;3;140;0
WireConnection;139;4;137;0
WireConnection;83;0;93;0
WireConnection;83;1;76;1
WireConnection;130;0;8;0
WireConnection;130;1;26;0
WireConnection;130;2;131;0
WireConnection;12;0;10;0
WireConnection;12;1;44;0
WireConnection;28;0;41;0
WireConnection;28;1;34;0
WireConnection;32;0;28;0
WireConnection;66;0;83;0
WireConnection;11;0;12;0
WireConnection;39;0;130;0
WireConnection;37;0;20;0
WireConnection;138;0;139;0
WireConnection;48;0;8;4
WireConnection;124;0;108;0
WireConnection;109;1;34;0
WireConnection;105;0;104;2
WireConnection;105;1;104;1
WireConnection;36;0;57;0
WireConnection;36;1;126;0
WireConnection;23;0;57;0
WireConnection;57;0;54;0
WireConnection;57;1;53;0
WireConnection;126;0;35;0
WireConnection;149;0;40;0
WireConnection;149;1;38;0
WireConnection;149;4;15;0
WireConnection;149;5;31;0
WireConnection;149;7;123;0
WireConnection;149;10;49;0
WireConnection;149;11;67;0
ASEEND*/
//CHKSM=D64BFE84C773EDA64D28EB78353A63D2E8B87D3B