
void LineDist_float(float3 p1, float3 p2, float3 pnt, float2 radius, out bool Out)
{
	// 3RD SOLUTION
	float3 dir = p2 - p1;

	dir = dir / sqrt(dot(dir, dir));

	float3 vet = pnt - p1;

	float t = max(dot(vet, dir),0);

	float3 dest = p1 + dir * t;

	float dst = distance(dest, pnt);

	//p1 worldCENTER
	//p2 CAMERA
	

	float nt = sqrt(dot(dest - p1, dest - p1)) / sqrt(dot(p2 - p1, p2 - p1));

	nt = clamp(nt, 0, 1);

	float accDst = radius.x + nt * (radius.y - radius.x);

	Out = dst < accDst;

}