// 
//  box.js

(function() {
    var _this;

    Box = function() {
        _this = this;
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
            this.updateProps();
        },

        updateProps: function() {
            var gunProps = Entities.getEntityProperties(this.entityID, ['position', 'rotation']);
            this.position = gunProps.position;
            this.rotation = gunProps.rotation;
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