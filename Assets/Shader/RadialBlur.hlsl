


void RadialBlur_float(UnityTexture2D MainTex, float2 UV, float3 Scale, float2 Position, out float3 RGB, out float A)
{
    float width = MainTex.texelSize.z;
    float height = MainTex.texelSize.w;
    
    float2 radiusUV = UV - Position;
    float r = length(radiusUV);
    radiusUV /= r;
    
    float samples = Scale.x;      // 샘플 개수
    float maxRadius = Scale.y;    // 최대 반경 (정규화 기준)
    float blurStrength = Scale.z; // 흐림 강도
    
    r = saturate(r / maxRadius);
    
    // 픽셀이 중심에서 멀수록 더 많이 퍼지게 만드는 이동량 계산
    float2 delta = -radiusUV * r * r * blurStrength / samples;
    
    float4 result = float4(0, 0, 0, 0);
    float2 uv = UV;
    int count = 0;
    
    // 중심 방향 샘플링
    [unroll(16)]
    for (int i = 0; i < samples; i++)
    {
        uv += delta;
        result += MainTex.Sample(MainTex.samplerstate, uv);
        count++;
    }

    result /= count;
    
    RGB = result.rgb;
    A = result.a;
}