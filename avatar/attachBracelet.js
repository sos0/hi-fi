//
//

var MODEL_URL = "https://dl.dropbox.com/s/1zsyxcr4n6yixnk/test_box_2.fbx";
var boxProperties = {
  type: 'Model',
  modelURL: MODEL_URL,
  dimensions: {
    x: 0.5,
    y: 0.5,
    z: 0.5
  },
  color: {
    red: 200,
    green: 0,
    blue: 20
  },
  shapeType: 'box',
  gravity: {
    x: 0,
    y: -5.0,
    z: 0
  },
  lifetime: -1,
  restitution: 0,
  collisionless: true
};

function attachEntityAtArm(jointName) {
    var jointID = MyAvatar.jointNames.indexOf(jointName);
    boxProperties.name = jointName; 
    boxProperties.parentJointIndex = jointID;
    boxProperties.position =  MyAvatar.getJointPosition(jointName);

    return Entities.addEntity(boxProperties);
}

Entities.addingEntity.connect(function(entityID){
  print ("Entity added.");
});
// attachEntityAtArm("RightForeArm");
MyAvatar.attach(MODEL_URL, "RightForeArm", {x: -0.0, y: -0.0, z: 0.0}, Quat.fromPitchYawRollDegrees(0, 0, 0), 1.5);
// Script.stop();