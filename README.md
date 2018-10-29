# Anna

This is a project that I started about a year ago and lost interest in. I do not believe that this software is necessary.

When opened in the Unity editor, Anna provides the user with an interface to load a dataset and construct an MLP, specifying the number and size of layers, to be used for prediction. A testbench is created that allows the user to train and test the NN on the loaded dataset for multiple iterations or epochs. 

Prior to creating a testbench, the loaded data can be cleaned, discretized, or normalized.  The target attribute can be specified at this time. Target attributes can be discrete or continuous, affecting only the number of outputs in the resulting MLP.

# Dependencies

Anna uses [Math.Net Numerics](www.numerics.mathdotnet.com) for its matrix representations and operations. Numerics is free, great, and is provided as a DLL in the project sources.
