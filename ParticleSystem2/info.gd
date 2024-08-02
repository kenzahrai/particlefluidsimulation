extends Label

@onready var FluidSimulation =  get_parent().get_parent()
var label_text = ""
var labels = []
var deltaTime = 1.67
@onready var cursor_visual = $CursorVisual
@onready var cursor_visual_checkbox = $CursorSimulationButton
# Called when the node enters the scene tree for the first time.
func _ready():
	deltaTime = FluidSimulation.deltaTime
	for i in range(FluidSimulation.particleCount):
		var new_label = Label.new()
		new_label.scale.x = 0.5
		new_label.scale.y = 0.5
		add_child(new_label)
		labels.append(new_label)
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#if $pausebutton.paused:
	#	FluidSimulation.deltaTime = 0
	#else:
	#	FluidSimulation.deltaTime = deltaTime
	label_text = ""
	label_text += "particle count: " + str(FluidSimulation.particleCount)+ "\n"
	label_text += "average particle density: " + str(FluidSimulation.average_density)+ "\n"
	label_text += "density at mouse point: " + str(FluidSimulation.CalculateDensityAtPoint(get_global_mouse_position()))+ "\n"
	
	var pressure_force_line = cursor_visual.get_node("Line2D")
	pressure_force_line.global_position = Vector2.ZERO
	cursor_visual.global_position = get_global_mouse_position()
	var pressure_force = FluidSimulation.CalculatePressureForceAtPoint(get_global_mouse_position())
	var circle = cursor_visual.get_node("Circle")
	label_text += "pressure force at mouse point: " + str(pressure_force)+ "\n"
	
	circle.scale.x = get_parent().get_parent().smoothingRadius
	circle.scale.y = get_parent().get_parent().smoothingRadius
	pressure_force_line.set_point_position(0,get_global_mouse_position())
	pressure_force_line.set_point_position(1,get_global_mouse_position() + pressure_force.normalized()*pressure_force.length()*1000 )
	if cursor_visual_checkbox.button_pressed:
		cursor_visual.show()
		for label1 in labels:
			label1.show()
	else:
		for label1 in labels:
			label1.hide()
		cursor_visual.hide()
	var information = FluidSimulation.debuginfo
	if !information.is_empty():
		for i in range(FluidSimulation.particleCount-1):
			var value = information[i].x *1000
			labels[i].text = "%.02f" % value
			labels[i].global_position = Vector2(information[i].y, information[i].z)
		text = label_text
	if (Input.is_action_pressed("rightclick")):
		FluidSimulation.InteractionForceCircle(get_global_mouse_position(), 100, 25)
	if (Input.is_action_pressed("leftclick")):
			FluidSimulation.InteractionForceCircle(get_global_mouse_position(), 100, -25)
	pass
