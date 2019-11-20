/*
  ==============================================================================

    This file was auto-generated!

    It contains the basic framework code for a JUCE plugin editor.

  ==============================================================================
*/

#include "PluginProcessor.h"
#include "PluginEditor.h"

//==============================================================================
Gps1AudioProcessorEditor::Gps1AudioProcessorEditor (Gps1AudioProcessor& p)
    : AudioProcessorEditor (&p), processor (p)
{
    // Make sure that before the constructor has finished, you've set the
    // editor's size to whatever you need it to be.
    setSize (400, 300);
    
    auto& params = processor.getParameters();
    AudioParameterFloat* gainParameter = (AudioParameterFloat*)params.getUnchecked(0);
    
    
    mGainControlSlider.setBounds(0,0,100,100);
    mGainControlSlider.setSliderStyle((Slider::SliderStyle::RotaryVerticalDrag));
    mGainControlSlider.setTextBoxStyle(Slider::NoTextBox, true, 0, 0);
    mGainControlSlider.setRange(gainParameter-> range.start, gainParameter-> range.end);
    mGainControlSlider.setValue(*gainParameter);
    mGainControlSlider.addListener(this);
    addAndMakeVisible(mGainControlSlider);
    
}

Gps1AudioProcessorEditor::~Gps1AudioProcessorEditor()
{
}

//==============================================================================
void Gps1AudioProcessorEditor::paint (Graphics& g)
{
    // (Our component is opaque, so we must completely fill the background with a solid colour)
    g.fillAll (getLookAndFeel().findColour (ResizableWindow::backgroundColourId));

    g.setColour (Colours::white);
    g.setFont (15.0f);
    g.drawFittedText ("Hello GPS!", getLocalBounds(), Justification::centred, 1);
}

void Gps1AudioProcessorEditor::resized()
{
    // This is generally where you'll want to lay out the positions of any
    // subcomponents in your editor..
}

void Gps1AudioProcessorEditor::sliderValueChanged(Slider *slider)
{
    auto& params = processor.getParameters();
    if (slider == &mGainControlSlider) {
        AudioParameterFloat* gainParameter = (AudioParameterFloat*)params.getUnchecked(0);
        *gainParameter = mGainControlSlider.getValue();
        DBG("Gain slider has changed");
    }
    DBG("slider value chage");
}
