extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().pressureMultiplier
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "pressure multiplier : " + str(get_parent().get_parent().pressureMultiplier)
	get_parent().get_parent().pressureMultiplier = value

	pass
