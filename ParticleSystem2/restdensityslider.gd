extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().restDensity
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "rest density : " + str(get_parent().get_parent().restDensity)
	get_parent().get_parent().restDensity = value

	pass
