extends SubViewportContainer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$SubViewport.size = get_viewport().size;


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
