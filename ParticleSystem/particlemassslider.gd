extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().particleMass
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "particle mass: " + str(get_parent().get_parent().particleMass)
	get_parent().get_parent().particleMass = value

	pass
