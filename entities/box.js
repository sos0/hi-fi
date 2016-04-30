// 
//  box.js

(function() {
    var _this;
    var TRIGGER_CONTROLS = [
        Controller.Hardware.Keyboard.1,
        Controller.Hardware.Keyboard.Space
    ];

    Box = function() {
        _this = this;
        this.fireSound = SoundCache.getSound("http://hifi-production.s3.amazonaws.com/tutorials/pistol/GUN-SHOT2.raw");
        this.fireVolume = 0.5;
        this.equipped = false;
    };
    Controller.hardwareChanged.connect(function(){
        print("Hardware changed");
    });

    Box.prototype = {
        startEquip: function(id, params) {
            this.equipped = true;
        },

        continueEquip: function(id, params) {
            if (!this.equipped) {
                return;
            }
            this.cast();
        },

        cast: function() {
            this.triggerValue = Controller.getValue(Controller.Standard.RT);

            if(this.triggerValue){
                Audio.playSound(this.fireSound, {
                    volume: this.fireVolume
                });

                var firework = Entities.addEntity({
                      name: "fireworks emitter",
                      position: MyAvatar.getJointPosition("RightForeArm"),
                      type: "ParticleEffect",
                      colorStart: hslToRgb({
                        h: Math.random(),
                        s: 0.5,
                        l: 0.7
                      }),
                      color: hslToRgb({
                        h: Math.random(),
                        s: 0.5,
                        l: 0.5
                      }),
                      colorFinish: hslToRgb({
                        h: Math.random(),
                        s: 0.5,
                        l: 0.7
                      }),
                      maxParticles: 10000,
                      lifetime: 20,
                      lifespan: randFloat(1.5, 3),
                      emitRate: randInt(500, 5000),
                      emitSpeed: randFloat(0.5, 2),
                      speedSpread: 0.2,
                      emitOrientation: Quat.fromPitchYawRollDegrees(randInt(0, 360), randInt(0, 360), randInt(0, 360)),
                      polarStart: 1,
                      polarFinish: randFloat(1.2, 3),
                      azimuthStart: -Math.PI,
                      azimuthFinish: Math.PI,
                      emitAcceleration: {
                        x: 0,
                        y: randFloat(-1, -0.2),
                        z: 0
                      },
                      accelerationSpread: {
                        x: Math.random(),
                        y: 0,
                        z: Math.random()
                      },
                      particleRadius: randFloat(0.001, 0.1),
                      radiusSpread: Math.random() * 0.1,
                      radiusStart: randFloat(0.001, 0.1),
                      radiusFinish: randFloat(0.001, 0.1),
                      alpha: randFloat(0.8, 1.0),
                      alphaSpread: randFloat(0.1, 0.2),
                      alphaStart: randFloat(0.7, 1.0),
                      alphaFinish: randFloat(0.7, 1.0),
                      textures: "http://ericrius1.github.io/PlatosCave/assets/star.png",
                    });

                Script.setTimeout(function() {
                  Entities.editEntity(firework, {
                    isEmitting: false
                  });
                }, 1000);
            }
        },

        releaseEquip: function(id, params) {
            this.equipped = false;
        },

        preload: function(entityID) {
            this.entityID = entityID;
        },
    };

    // entity scripts always need to return a newly constructed object of our type
    return new Box();
});