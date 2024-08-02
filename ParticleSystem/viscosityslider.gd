extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().viscosity
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "viscosity : " + str(get_parent().get_parent().viscosity)
	get_parent().get_parent().viscosity = value

	pass
