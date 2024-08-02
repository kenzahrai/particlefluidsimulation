extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().nearpressureMultiplier
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "near pressure multiplier : " + str(get_parent().get_parent().nearpressureMultiplier)
	get_parent().get_parent().nearpressureMultiplier = value

	pass
