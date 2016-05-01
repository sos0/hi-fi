(function() {
	var data = [{"type":0,"tiles":[],"name":"world.unity","objects":[{"scale":{"y":1,"x":1,"z":1,"w":1},"position":{"y":0,"x":0,"z":0,"w":0},"n":"test_box_2","id":2071402159,"rotation":{"y":0,"x":0,"z":0,"w":0}},{"scale":{"y":1,"x":1,"z":1,"w":1},"position":{"y":0,"x":0,"z":0,"w":0},"n":"test_box_2","id":668739892,"rotation":{"y":0,"x":0,"z":0,"w":0}}]}];

  for(var i = 0; i < data[0].objects.length; i++) {
  	var object = data[0].objects[i];

  	var properties = {
	  type: "Sphere",
	  position: object.position
	};
	Ent = Entities.addEntity(properties);
  }


})();