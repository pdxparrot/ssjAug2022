mod hello;

use gdnative::prelude::*;

use hello::*;

fn init(handle: InitHandle) {
    handle.add_class::<HelloWorld>();
}

godot_init!(init);
