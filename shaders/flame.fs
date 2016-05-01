#line 102

uniform vec4 iControlMode = vec4(0.0);

// Grabbed from the binaries, the code that exposes the TransformCamera
struct TransformCamera {
    mat4 _view;
    mat4 _viewInverse;
    mat4 _projectionViewUntranslated;
    mat4 _projection;
    mat4 _projectionInverse;
    vec4 _viewport;
};

layout(std140) uniform transformCameraBuffer {
    TransformCamera _camera;
};
TransformCamera getTransformCamera() {
    return _camera;
}

vec2 windowToClipSpace(TransformCamera cam, vec2 viewportPos) {
    vec2 viewportSize = cam._viewport.zw;
    // Detect stereo case
    // if (gl_FragCoord.x > viewportSize.x) {
    //     return ((viewportPos - vec2(viewportSize.x,0.0) - cam._viewport.xy) / cam._viewport.zw) * 2.0 - vec2(1.0); 
    // } else {
        return ((viewportPos - cam._viewport.xy) / cam._viewport.zw) * 2.0 - vec2(1.0); 
    // }
}

vec3 clipToEyeSpace(TransformCamera cam, vec3 clipPos) {
    return vec3(cam._projectionInverse * vec4(clipPos.xyz, 1.0));
}
vec3 eyeToWorldSpace(TransformCamera cam, vec3 eyePos) {
    return vec3(cam._viewInverse * vec4(eyePos.xyz, 1.0));
}

vec3 eyeToWorldSpaceDir(TransformCamera cam, vec3 eyePos) {
    return vec3(cam._viewInverse * vec4(eyePos.xyz, 0.0));
}



/////////////////////////////////
/////////////////////////////////
// From: https://www.shadertoy.com/view/MdX3zr
/////////////////////////////////
/////////////////////////////////

float noise(vec3 p) //Thx to Las^Mercury
{
    vec3 i = floor(p);
    vec4 a = dot(i, vec3(1., 57., 21.)) + vec4(0., 57., 21., 78.);
    vec3 f = cos((p-i)*acos(-1.))*(-.5)+.5;
    a = mix(sin(cos(a)*a),sin(cos(1.+a)*(1.+a)), f.x);
    a.xy = mix(a.xz, a.yw, f.y);
    return mix(a.x, a.y, f.z);
}

float sphere(vec3 p, vec4 spr)
{
    return length(spr.xyz-p) - spr.w;
}

float flame(vec3 p)
{
    float d = sphere(p*vec3(1.,.5,1.), vec4(.0,-1.,.0,1.));
    return d + (noise(p+vec3(.0,iGlobalTime*-3.,.0)) + noise(p*3.)*.5)*.25*(p.y) ;
}

float scene(vec3 p)
{
    p = p * 3;
    return min(100.-length(p) , abs(flame(p)) );
}

// vec4 raymarch(vec3 org, vec3 dir)
// {
//     float d = 0.0, glow = 0.0, eps = 0.04;
//     vec3  p = org;
//     bool glowed = false;
    
//     for(int i=0; i<32; i++)
//     {
//         d = scene(p) + eps;
//         p += d * dir;
//         if( d>eps )
//         {
//             if(flame(p) < .0)
//                 glowed=true;
//             if(glowed)
//                 glow = float(i)/40.;
//         }
//     }
//     return vec4(p,glow);
// }

// void mainImage( out vec4 fragColor, in vec2 fragCoord )
// {
//     vec2 v = -1.0 + 2.0 * fragCoord.xy / iResolution.xy;
//     v.x *= iResolution.x/iResolution.y;
    
//     vec3 org = vec3(0., -2., 4.);
//     vec3 dir = normalize(vec3(-v.x*1.6, -v.y,-1));
    
//     vec4 p = raymarch(org, dir);
//     float glow = p.w;
    
//     vec4 col = mix(vec4(1.,.5,.1,1.), vec4(0.1,.5,1.,1.), p.y*.02+.4);
    
//     fragColor = mix(vec4(0.), col, pow(glow*2.,4.));
//     //fragColor = mix(vec4(1.), mix(vec4(1.,.5,.1,1.),vec4(0.1,.5,1.,1.),p.y*.02+.4), pow(glow*2.,4.));

// }

////////////////////////////////////////////////////////////////
// Math Primitives
////////////////////////////////////////////////////////////////

void evalRayToBoxCords( in vec3 rayO, in vec3 rayD, out vec3 T0, out vec3 T1) {
    vec3 C0 = vec3(-1.0);
    vec3 C1 = vec3(1.0);

    T0 = (C0 - rayO) / rayD;
    T1 = (C1 - rayO) / rayD;
}

vec2 evalRayToBoxInOut(in vec3 rayO, in vec3 rayD) {
    vec3 T0;
    vec3 T1;
    evalRayToBoxCords(rayO, rayD, T0, T1);

    vec3 Tm = min(T0, T1);
    vec3 TM = max(T0, T1);
    vec2 io;

    if (Tm.x > Tm.y) {
        if (Tm.x > Tm.z)
            io.x = (Tm.x);
        else
            io.x = (Tm.z);
    } else {
        if (Tm.y > Tm.z)
            io.x = (Tm.y);
        else
            io.x = (Tm.z);
    }

    if (TM.x < TM.y) {
        if (TM.x < TM.z)
            io.y = (TM.x);
        else
            io.y = (TM.z);
    } else {
        if (TM.y < TM.z)
            io.y = (TM.y);
        else
            io.y = (TM.z);
    }
    return io;
}


////////////////////////////////////////////////////////////////
// Volume Shape
////////////////////////////////////////////////////////////////

const float VOLUME_MAX_LEN = 2.0;
const float VOLUME_SCALE = 1.0;

// vec3 worldToVolume(vec3 wpos) {
//     return (wpos -  iWorldPosition) * VOLUME_SCALE / (0.5 * iWorldScale);
// }
// vec4 worldToVolumeAndRadius(vec4 wpos) {
//     return vec4((wpos.xyz -  iWorldPosition) * VOLUME_SCALE / (0.5 * iWorldScale), wpos.w);
// }

void evalBoxVolumeRay(in vec3 fragPos, out vec3 rayIn, out vec3 rayOut, out vec3 rayDir, out float rayLen) {
    TransformCamera camera = getTransformCamera();

    vec3 oriPosW = iWorldPosition;
    
    vec2 pixPos = windowToClipSpace(camera, gl_FragCoord.xy);
    vec3 eyePosE = clipToEyeSpace(camera, vec3(pixPos, 0.0));

    vec3 eyePosW = eyeToWorldSpace(camera, eyePosE);
    vec3 eyeDirW = eyeToWorldSpaceDir(camera, eyePosE);

    vec3 fragPosV = _position.xyz * VOLUME_SCALE; 

    vec3 oriToEyeW = eyePosW - oriPosW;
    vec3 eyePosV = oriToEyeW * VOLUME_SCALE / (0.5 * iWorldScale);
    
    rayDir = normalize(eyeDirW);

    vec2 rayIO = evalRayToBoxInOut(eyePosV, rayDir);

    // detect if inside the volume
    if (rayIO.x < 0)
        rayIO.x = 0;

    rayIn = eyePosV + rayIO.x * rayDir;
    rayOut = eyePosV + rayIO.y * rayDir;
    rayLen = rayIO.y - rayIO.x;
}

////////////////////////////////////////////////////////////////
// Volume Scene
////////////////////////////////////////////////////////////////

uniform vec4 iHandPos = vec4(0.0, 0.0, 0.0, -1.0);

vec4 fetchSimpleScene(vec3 pos, float time) {
    pos.y += 1.0;
    pos.y *= 0.5;
    // float ltime = 0.75;// + 0.25 * time;
    float intensity = 1 - 1.8*scene(pos);
    //ltime * 1.0 - dot(pos, pos);

    vec4 col = intensity * mix(vec4(0.5,0.03,0.03,1), vec4(1.,0.1,0,1), pos.y*2);

    return col;
}

////////////////////////////////////////////////////////////////
// Ray tracing
////////////////////////////////////////////////////////////////

const float MAX_NUM_STEPS = 100;
const float STEP_LEN = VOLUME_MAX_LEN / MAX_NUM_STEPS; // worse case is 2 / MAX_NUM_STEPS
const float ALPHA_TRESHOLD = 0.1;

bool blendSample(vec4 value, inout vec4 result) {
    if (value.a >= ALPHA_TRESHOLD) {      
        result.rgb += value.rgb;
        result.rgb = min(vec3(1,1,1), result.rgb);
        result.a += (1.0-result.a)*value.a;

        if (result.a >= 1.0) {
            result.a = 1;
            return true;
        }
    } 
    return false;  
}

vec4 rayTrace(vec3 rayIn, vec3 rayDir, float rayLen, vec3 rayOut, float time) {
    float numSteps = min(floor(rayLen / STEP_LEN), float(MAX_NUM_STEPS));
    float stepLen = STEP_LEN;

    vec4 result = vec4(0.0);

    vec4 value = vec4(0.0);
    vec3 samplePos = rayIn;
    int tracedSteps = 0;
    for (int i=0; i < numSteps; i++) {
        value = fetchSimpleScene(samplePos, time);
        // value = fetchVolume(samplePos, time);
        samplePos = rayIn + i * stepLen * rayDir;
        if (blendSample(value, result)) {
            i = int(numSteps);
        }
        tracedSteps ++;
    }

    if (result.a < 1) {
        value = fetchSimpleScene(rayIn + rayLen * rayDir, time);
        // value = fetchVolume(rayIn + rayLen * rayDir, time);
        blendSample(value, result);
    }

   // if ( gl_FragCoord.x > 900)
       return result;
    return vec4(vec3(tracedSteps / float(numSteps)), 1.0);
}

////////////////////////////////////////////////////////////////
// Setup & Main
////////////////////////////////////////////////////////////////

vec4 getProceduralColor() {
    // Define time
     float time = cos(iControlMode.y * iGlobalTime);

    // Define volume ray
    vec3 rayInV;
    vec3 rayOutV;
    vec3 rayDirV;
    float rayLenV;
    evalBoxVolumeRay(_position.xyz, rayInV, rayOutV, rayDirV, rayLenV);


    // now step in the volume
    // vec4 rayColor = raymarch(vec3(0,0,0), rayInV);
    vec4 rayColor = rayTrace(rayInV, rayDirV, rayLenV, rayOutV, time);
    if (rayColor.a < 0.001) {
       discard;
    }

    return rayColor;
}


float getProceduralColors(inout vec3 diffuse, inout vec3 specular, inout float shininess) {
    vec4 result = getProceduralColor();
//    diffuse = vec3(1.0);
    diffuse = result.xyz;
    specular = vec3(1.0);
    shininess = 1;
    return result.a;
}


