using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UV_DLP_3D_Printer.Slicing;
namespace UV_DLP_3D_Printer
{
    public class GCodeGenerator
    {
        private GCodeGenerator() 
        {
        }
        /*
         GCode Process for building 3d DVP UV objects
         
         * <Build Start>
         * <Slicing Comments> comments containing the slicing and building parameters
         * <Header Code> start.gcode - commands from this 
         * file are inserted, they contain whatever intiailization sequences are need to initialize the machine
         * at this point, the build tray is in position to start printing a layer
         * <Layer Start>
         * Display the correct image slice for the current layer
         * Delay for <Layertime> to expose the UV resin
         * <Layer End>
         
         
        */

        // here we prepare the gcode preprocessor and fill all nessesary variables 
        protected static PreProcessor PreparePreprocessor(SliceFile sf, MachineConfig pi)
        {
            PreProcessor pp = new PreProcessor();
            pp.SetVar("$LayerThickness", sf.m_config.ZThick); // the thickenss of the layer in mm
            pp.SetVar("$ZLiftDist", sf.m_config.liftdistance); // how far we're lifting
            pp.SetVar("$ZLiftRate", sf.m_config.liftfeedrate); // the rate at which we're lifting
            pp.SetVar("$ZBottomLiftRate", sf.m_config.bottomliftfeedrate); // the rate at which we're lifting for the bottom layers
            pp.SetVar("$ZRetractRate", sf.m_config.liftretractrate); // how fast we'r retracting
            pp.SetVar("$SlideTiltVal", sf.m_config.slidetiltval); // any used slide / tilt value on the x axis
            pp.SetVar("$BlankTime", sf.m_config.blanktime_ms); // how long to show the blank in ms
            pp.SetVar("$LayerTime", sf.m_config.layertime_ms); // total delay for a layer for gcode commands to complete - not including expusre time
            pp.SetVar("$FirstLayerTime", sf.m_config.firstlayertime_ms); // time to expose the first layers in ms
            pp.SetVar("$NumFirstLayers", sf.m_config.numfirstlayers); // number of first layers
            return pp;
        }

        /*
         This is the main function to generate gcode files for the 
         * already sliced model
         */
        public static GCodeFile Generate(SliceFile sf, MachineConfig pi) 
        {
            String gcode;
            StringBuilder sb = new StringBuilder();
            PreProcessor pp = PreparePreprocessor(sf, pi);
            double zdist = 0.0;
           // double feedrate = pi.ZMaxFeedrate; // 10mm/min
            double liftfeed = sf.m_config.liftfeedrate;
            double bottomliftfeed = sf.m_config.bottomliftfeedrate;

            double retractfeed = sf.m_config.liftretractrate;
            double zdir = 1.0; // assume a bottom up machine
            int numbottom = sf.m_config.numfirstlayers;

            if (sf.m_config.direction == SliceBuildConfig.eBuildDirection.Top_Down) 
            {
                zdir = -1.0;// top down machine, reverse the z direction
            }
            pp.SetVar("$ZDir", zdir);

            // append the build parameters as reference
            sb.Append(sf.m_config.ToString());
            sb.Append(";Number of Slices        =  " + sf.NumSlices.ToString() + "\r\n");

            sb.Append(pi.ToString());//add the machine build parameters string

            // append the header
            sb.Append(pp.Process(sf.m_config.HeaderCode));

            zdist = sf.m_config.ZThick;

            String firstlayerdelay = ";<Delay> " + sf.m_config.firstlayertime_ms + " \r\n";
            String layerdelay = ";<Delay> " + sf.m_config.layertime_ms + " \r\n";
            String blankdelay = ";<Delay> " + sf.m_config.blanktime_ms + " \r\n";

            String preSliceGCode = pp.Process(sf.m_config.PreSliceCode);
            String LiftGCode = pp.Process(sf.m_config.LiftCode);

            int numslices = sf.NumSlices;
            if (sf.m_modeltype == SliceFile.ModelType.eResinTest1)
                numslices -= 10;

            // for a per-slice basis, we're going to re-evaluate the prelift and lift gcode
            // doing this will allow us to have per-slice determinations based on slice index.
            int c;
            for (c = 0; c < numslices; c++)
            {
                pp.SetVar("$CURSLICE", c);
                preSliceGCode = pp.Process(sf.m_config.PreSliceCode);
                //pp.SetVar("$CURSLICE", c);
                sb.Append(preSliceGCode);//add in the pre-slice code
                // this is the marker the BuildManager uses to display the correct slice
                sb.Append(";<Slice> " + c + " \r\n");
                // add a pause for the UV resin to be set using this image
                if (c < numbottom)// check for the bottom layers
                {
                    sb.Append(firstlayerdelay);
                }
                else
                {
                    sb.Append(layerdelay);
                }
                sb.Append(";<Slice> Blank \r\n"); // show the blank layer
                LiftGCode = pp.Process(sf.m_config.LiftCode); // re-run the lift code
                sb.Append(LiftGCode); // append the pre-lift codes
            }

            // special ending on resin test model slicing
            if (sf.m_modeltype == SliceFile.ModelType.eResinTest1)
            {
                for (; c < sf.NumSlices; c++)
                {
                    sb.Append(";<Slice> " + c + " \r\n");
                    // add a pause for the UV resin to be set using this image
                    if (c == sf.NumSlices -1) // set minimus exposure time on final layer
                        sb.Append(";<Delay> " + sf.m_config.minExposure + " \r\n");
                    else
                        sb.Append(";<Delay> " + sf.m_config.exposureStep + " \r\n");
                }
                sb.Append(";<Slice> Blank \r\n"); // show the blank layer
                LiftGCode = pp.Process(sf.m_config.LiftCode); // re-run the lift code
                sb.Append(LiftGCode); // append the pre-lift codes
            }



            //append the footer
            sb.Append(pp.Process(sf.m_config.FooterCode));
            gcode = sb.ToString();
            GCodeFile gc = new GCodeFile(gcode);
            return gc;
        }
    }
}
