
void LineDist_float(float3 p1, float3 p2, float3 pnt, out float Out)
{
	float num = dot((p1 - pnt), (p2 - p1));
	float den = sqrt((p2 - p1) * (p2 - p1));

	float t = clamp(- num / den,0,1);
	

	Out = distance(pnt, p1 + (p2 - p1) * t);
}