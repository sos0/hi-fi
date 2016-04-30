//
//

var MODEL_URL = "https://dl.dropboxusercontent.com/s/1zsyxcr4n6yixnk/test_box_2.fbx";
var boxProperties = {
  type: 'Model',
  modelURL: MODEL_URL,
  parentID: MyAvatar.sessionUUID,
  dimensions: {
    x: 0.1,
    y: 0.1,
    z: 0.1
  },
  color: {
    red: 200,
    green: 0,
    blue: 20
  },
  shapeType: 'box',
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
attachEntityAtArm("RightForeArm");

// MyAvatar.attach(MODEL_URL, "RightForeArm", {x: -0.0, y: -0.0, z: 0.0}, Quat.fromPitchYawRollDegrees(0, 0, 0), 0.2);
Script.stop();