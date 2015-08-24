// PRUSA Mendel
// RAMPS-holder (modified endstop holder)
// Used to attach RAMPS to 8mm rods
// GNU GPL v3
// Endstop holder by:
// Josef Prusa
// josefprusa@me.com
// prusadjs.cz
// RAMPS-holder by:
// Michiel Haisma
// michielhaisma@hotmail.com
// http://blog.stygia.nl
//
// http://github.com/prusajr/PrusaMendel

m8_diameter = 9; 
m8_nut_diameter = 14.1;

m4_diameter = 4.5;
m4_nut_diameter = 9;

m3_diameter = 3.6;
m3_nut_diameter = 5.3;
m3_nut_diameter_horizontal = 6.1;

/**
 * @id endstop-holder
 * @name Endstop holder
 * @category Printed
 * @using 1 m3x20xhex
 * @using 1 m3nut
 * @using 2 m3washer
 */
module endstop(){
outer_diameter = m8_diameter/2+3.3;
screw_hole_spacing = 50;
opening_size = m8_diameter-1.5; //openingsize

difference(){
	union(){
		translate([outer_diameter, outer_diameter, 0]) cylinder(h =12, r = outer_diameter, $fn = 20);
		translate([outer_diameter, 0, 0]) cube([15.5,outer_diameter*2,12]);
		translate([-62, 0, 0]) cube([70, 6, 12]);
		translate([17, 17.5, 6]) rotate([90, 0, 0]) #cylinder(h = 5, r = 5.77, $fn = 6);
	}

	translate([9, outer_diameter-opening_size/2, 0]) cube([18,opening_size,20]);
	translate([outer_diameter, outer_diameter, 0]) cylinder(h =20, r = m8_diameter/2, $fn = 18);

	//Securing hole
	translate([17, 17, 6]) rotate([90, 0, 0]) cylinder(h =20, r = m3_diameter/2, $fn = 10);
	translate([17, 19.5, 6]) rotate([90, 0, 0]) #cylinder(h =6, r = m3_nut_diameter_horizontal/2, $fn = 6);

	translate([17, 17, 6]) rotate([90, 0, 0]) #cylinder(h =20, r = m3_diameter/2, $fn = 10);
	translate([-4, 17, 6]) rotate([90, 0, 0]) cylinder(h =20, r = m3_diameter/2, $fn = 10);
	translate([-(4+screw_hole_spacing), 17, 6]) rotate([90, 0, 0]) cylinder(h =20, r = m3_diameter/2, $fn = 10);
}
}
endstop();

