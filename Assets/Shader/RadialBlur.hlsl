void RadialBlur_float(UnityTexture2D MainTex, float2 UV, float3 Scale, float2 Position, out float3 RGB, out float A)
{
    float width = MainTex.texelSize.z;
    float height = MainTex.texelSize.w;
    
    float2 radiusUV = UV - float2(0.5f, 0.5f);
    float r = length(radiusUV);
    radiusUV /= r;
    
    r = saturate(r / Scale.y);
    
    float2 delta = -radiusUV * r * r * Scale.z / Scale.x;
    
    float4 result = float4(0, 0, 0, 0);
    float2 uv = UV;
    int count = 0;
    
    [unroll(16)]
    for (int i = 0; i < Scale.x; i++)
    {
        uv += delta;
        result += MainTex.Sample(MainTex.samplerstate, uv);
        count++;
    }

    result /= count;
    
    RGB = result.rgb;
    A = result.a;
}