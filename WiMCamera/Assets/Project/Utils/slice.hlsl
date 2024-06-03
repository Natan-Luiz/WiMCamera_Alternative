
void MySliceFunction_float(float4 sliceNormal, float4 sliceCenter, float sliceOffset, float4 worldPos, out float Out)
{
	float3 adjustedCentre = sliceCenter + sliceNormal * sliceOffset;
	float3 offsetToSliceCentre = adjustedCentre - worldPos;
	float d = dot(offsetToSliceCentre, sliceNormal);
	clip(d);

	Out = 1;
}