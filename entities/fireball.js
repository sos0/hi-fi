(function() {

    var _this = this;
    _this.COLLISION_COOLDOWN_TIME = 1000;

    _this.preload = function(entityID) {

        //set our id so other methods can get it. 
        _this.entityID = entityID;
        //load the mooing sound

        //variables we will use to keep track of when to reset the cow
        _this.timeSinceLastCollision = 0;
        _this.shouldUntipCow = true;
    }

    _this.collisionWithEntity = function(myID, otherID, collisionInfo) {
        //we dont actually use any of the parameters above, since we don't really care what we collided with, or the details of the collision. 

        //5 seconds after a collision, upright the cow.  protect from multiple collisions in a short timespan with the 'shouldUntipCow' variable
        if (_this.shouldUntipCow) {
            //in Hifi, preface setTimeout with Script.setTimeout
            Script.setTimeout(function() {
                _this.untipCow();
                _this.shouldUntipCow = true;
            }, _this.COLLISION_COOLDOWN_TIME);
        }

        _this.shouldUntipCow = false;
    }

    _this.untipCow = function() {
        // keep yaw but reset pitch and roll
        print("untip")
    }

});