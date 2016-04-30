// 
//  box.js

(function() {
    var _this;

    Box = function() {
        _this = this;
        this.fireSound = SoundCache.getSound("http://hifi-production.s3.amazonaws.com/tutorials/pistol/GUN-SHOT2.raw");
        this.fireVolume = 0.5;
        this.equipped = false;
    };

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
            this.triggerValue = Controller.getValue(Controller.Hardware.Keyboard.1);

            if(this.triggerValue){
                Audio.playSound(this.fireSound, {
                    volume: this.fireVolume
                });
            }
        },

        releaseEquip: function(id, params) {
            this.equipped = false;
        },

        preload: function(entityID) {
            this.entityID = entityID;
        }
    };

    // entity scripts always need to return a newly constructed object of our type
    return new Box();
});