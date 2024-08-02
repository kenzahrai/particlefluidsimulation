extends HSlider
@onready var label = $Label

# Called when the node enters the scene tree for the first time.
func _ready():
	value = get_parent().get_parent().deltaTime
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	label.text = "delta time : " + str(get_parent().get_parent().deltaTime)
	get_parent().get_parent().deltaTime = value

	pass
