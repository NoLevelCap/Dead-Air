using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapGen : MonoBehaviour {

	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	[Range(0.01f, 0.2f)]
	public float FCP = 0.1f;
	[Range(0.001f, 0.1f)]
	public float SCP = 0.2f;
	public int oct = 1;
	public int oct2 = 1;
	public int socts = 1;

	public float scale = 5F;
	private Texture2D noiseTex;
	private Color[] pix, n1, FIRST, SECOND;

	void Start() {
		Debug.Log ("INITED");
		noiseTex = new Texture2D(pixWidth, pixHeight);		
		noiseTex.filterMode = FilterMode.Point;
		noiseTex.anisoLevel = 0;
		pix = new Color[noiseTex.width * noiseTex.height];
		n1 = new Color[noiseTex.width * noiseTex.height];
		FIRST = new Color[noiseTex.width * noiseTex.height];
		SECOND = new Color[noiseTex.width * noiseTex.height];
	}

	void Fill(Color[] am, float r, float g, float b){
		for (int x = 0; x < pixWidth; x++) {
			for (int y = 0; y < pixHeight; y++) {
				am [y * noiseTex.width + x] = new Color (r, g, b);
			}
		}
	}

	void Apply(bool ff){
		for (int x = 0; x < pixWidth; x++) {
			for (int y = 0; y < pixHeight; y++) {
				if(pix [y * noiseTex.width + x].grayscale == 1f || ff){
					pix [y * noiseTex.width + x] = n1 [y * noiseTex.width + x];
				}
			}
		}
	}

	void CenterPoint(float width){
		int cX = Mathf.RoundToInt (pixWidth/2f);
		int cY = Mathf.RoundToInt (pixHeight/2f);
		int WW = Mathf.RoundToInt (pixWidth * width);
		for (int x = cX-WW; x < cX+WW; x++) {
			for (int y = cY-WW; y < cY+WW; y++) {
				pix [y * noiseTex.width + x] = new Color (0f, 0f, 0f);
			}
		}
	}

	void CircularNoise(int octaves){
		for (int o = 0; o < octaves; o++) {
			for (int x = 0; x < pixWidth; x++) {
				for (int y = 0; y < pixHeight; y++) {
					float xCoord = xOrg + (noiseTex.width*o) + (float)(x) / noiseTex.width * scale;
					float yCoord = yOrg + (noiseTex.height*o) + (float)(y) / noiseTex.height * scale;
					float sample = Mathf.PerlinNoise(xCoord, yCoord);
					//float sample = Random.value;
					float average = averagePoint (x, y);

					//Debug.Log (average + "/" + sample + "/" + (Mathf.PerlinNoise(xCoord, yCoord)));

					if (average > sample && average > .2f) {
						n1[y * noiseTex.width + x] = new Color(0f, 0f, 0f);
					}

					//pix[y * noiseTex.width + x] = new Color(sample, sample, sample);
				}
			}
			Apply (false);
		}
	}

	float averagePoint(int x, int y){
		int a = 0;
		int t = 0;
		for (int mx = -1; mx < 2; mx++) {
			for (int my = -1; my < 2; my++) {
				if(inBounds(x+mx, y+my)){
					t++;
					if(pix [(y+my) * noiseTex.width + (x+mx)].grayscale == 0f){
						a++;
					}
				}
			}
		}
		return (float) a / (float) t;
	}

	bool inBounds(int x, int y){
		if(x >= 0 && y < pixHeight && x < pixWidth && y >= 0){
			return true;
		}
		return false;
	}

	void Smooth(int octaves){
		for (int o = 0; o < octaves; o++) {
			Copy (n1, pix);
			for (int x = 0; x < pixWidth; x++) {
				for (int y = 0; y < pixHeight; y++) {
					bool smthd = false;
					if(!smthd && ShouldTurnWhite(x, y)){
						smthd = true;
						n1 [(y) * noiseTex.width + (x)] = new Color (1f, 1f, 1f);
					}

					if(!smthd && ShouldTurnBlack(x, y)){
						smthd = true;
						n1 [(y) * noiseTex.width + (x)] = new Color (0f, 0f, 0f);
					}
				}
			}
			Apply (true);
		}
	}

	bool ShouldTurnWhite(int x, int y){
		if(isBlack(x, y)){
			if(!isBlack(x+1, y) && !isBlack(x-1, y)){
				return true;
			}

			if(!isBlack(x, y+1) && !isBlack(x, y-1)){
				return true;
			}
				
		}
		return false;
	}

	bool ShouldTurnBlack(int x, int y){
		if(!isBlack(x, y)){
			if(!isBlack(x-1, y-1) && isBlack(x, y-1) && isBlack(x+1, y-1) 
				&& !isBlack(x-1, y) && isBlack(x+1, y) 
				&& !isBlack(x-1, y+1) && !isBlack(x, y+1) && !isBlack(x+1, y+1)){
				return true;
			}

			if(isBlack(x-1, y-1) && isBlack(x, y-1) && !isBlack(x+1, y-1) 
				&& isBlack(x-1, y) && !isBlack(x+1, y) 
				&& !isBlack(x-1, y+1) && !isBlack(x, y+1) && !isBlack(x+1, y+1)){
				return true;
			}

			if(!isBlack(x-1, y-1) && !isBlack(x, y-1) && !isBlack(x+1, y-1) 
				&& isBlack(x-1, y) && !isBlack(x+1, y) 
				&& isBlack(x-1, y+1) && isBlack(x, y+1) && !isBlack(x+1, y+1)){
				return true;
			}

			if(!isBlack(x-1, y-1) && !isBlack(x, y-1) && !isBlack(x+1, y-1) 
				&& !isBlack(x-1, y) && isBlack(x+1, y) 
				&& !isBlack(x-1, y+1) && isBlack(x, y+1) && isBlack(x+1, y+1)){
				return true;
			}
		}
		return false;
	}

	bool isBlack(int x, int y){
		if (inBounds (x, y)) {
			if (pix [(y) * noiseTex.width + (x)].grayscale == 0f) {
				return true;
			}
		}
		return false;
	}

	void Copy(Color[] a, Color[] b){
		for (int x = 0; x < pixWidth; x++) {
			for (int y = 0; y < pixHeight; y++) {
				a [y * noiseTex.width + x] = b [y * noiseTex.width + x];
			}
		}
	}

	void SqaureOff(int rev){
		for (int o = 0; o < rev; o++) {
			bool ns;
			int x1, y1, x2, y2;
			do{
				ns = true;
				x1 = 0;
				x2 = 0;
				y1 = 0;
				y2 = 0;


				for (int x = 0; x < pixWidth; x++) {
					for (int y = 0; y < pixHeight; y++) {
						if(isBlack(x, y)){
							x1 = x;
							y1 = y;
							break; 
						}
					}
				}


				for (int y = 0; y < pixHeight; y++) {
					for (int x = pixWidth-1; x >= 0; x--) {
						if(isBlack(x, y)){
							x2 = x;
							y2 = y;
							break;
						}
					}
				}

				for (int x = x1; x < x2; x++) {
					for (int y = y1; y < y2; y++) {
						if(!isBlack(x, y)){
							ns = false;
							break;
						}
					}
					if(!ns){
						break;
					}
				}
			} while(!ns);

			for (int x = x1; x < x2; x++) {
				for (int y = y1; y < y2; y++) {
					pix [(y) * noiseTex.width + (x)] = new Color (0f, 0f, 0f);
				}
			}
		}
	}

	void Multiply(float amount){
		for (int x = 0; x < pixWidth; x++) {
			for (int y = 0; y < pixHeight; y++) {
				float sample = 1f - ((1f - pix [(y) * noiseTex.width + (x)].r) * amount);
				pix [(y) * noiseTex.width + (x)] = new Color (sample, sample, sample);
			}
		}

	}



	void CalcNoise(Texture2D tex, int seed) {
		Random.InitState(seed);


		Fill (pix, 1f, 1f, 1f);
		Fill (n1, 1f, 1f, 1f);
		CenterPoint (FCP);

		CircularNoise (oct);

		Smooth (socts);

		Multiply (0.5f);

		Copy (FIRST, pix);

		Fill (pix, 1f, 1f, 1f);
		Fill (n1, 1f, 1f, 1f);
		CenterPoint (SCP);

		CircularNoise (oct2);

		Smooth (socts);

		Copy (n1, FIRST);

		Apply (false);

		tex.SetPixels(pix);
		tex.Apply ();
	}

	private float distance(float x1, float y1, float x2, float y2) {
		return (float)(Mathf.Sqrt(Mathf.Pow(Mathf.Abs(x1 - x2), 2) + Mathf.Pow(Mathf.Abs(y1 - y2), 2) ));
	}

	void Update() {
	}

	public Texture2D NoiseTex {
		get {
			CalcNoise (noiseTex, 9);
			return this.noiseTex;
		}
	}

	public Color[] MapColors {
		get {
			CalcNoise (noiseTex, 9);
			return this.pix;
		}
	}



}
