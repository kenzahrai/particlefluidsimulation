extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().smoothingRadius
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "smoothing radius : " + str(get_parent().get_parent().smoothingRadius)
	get_parent().get_parent().smoothingRadius = value
	pass
