extends MultiMeshInstance3D


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var ind : int = 0
	for i in range(20):
		for j in range(20):
			multimesh.set_instance_transform(ind,Transform3D(Basis(),Vector3(i,0,j)))
			ind += 1


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta: float) -> void:
	pass
