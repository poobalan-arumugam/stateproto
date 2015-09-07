from parseStateProtoFile import *
from StateTreeModel import *
from codegens.pygen import *
from codegens.jsgen import *
from codegens.stgen import *

def tag(tagName, tagValue):
    return "<%(tagName)s>%(tagValue)s</%(tagName)s>" % locals()

def walkStateTree_Table(rootNode, arg):
    name = rootNode.name()
    state = rootNode.state()
    entry = state.entry
    exit = state.exit
    print "<table id='", name, "'><tr><td>"
    print "<div ", style," border='2' id='", name, "'><tr><td>"
    print tag("h1", name)
    print tag("p", entry)
    print tag("p", exit)
    rootNode.do(walkStateTree, arg)
    print "</td></tr></table>"

def walkStateTree_Div(rootNode, zorder):
    name = rootNode.name()
    state = rootNode.state()
    entry = state.entry
    exit = state.exit
    style = "style='z-index:%s;position:absolute;left:%s;top:%s;width:%s;height:%s'" % (zorder, state.left, state.top, state.width, state.height,)
    classId = " class='clsdiv" + str(zorder) + "' "
    print "<div ", style, classId, " id='", name, "'>"
    #print "<table ", style," id='", name, "'><tr><td>"
    print tag("h1", name)
    print tag("p", entry)
    print tag("p", exit)
    #print "</td></tr></table>"
    print "</div>"
    rootNode.do(walkStateTree, zorder + 1)

def walkStateTree(rootNode, arg):
    #walkStateTree_Table(rootNode, None)
    walkStateTree_Div(rootNode, arg)

if __name__ == "__main__":
    fileList = ["phoneSim1.sm1", 
        r"S:\oss\Development\MurphyPA\Modelling\H2D\doc\Hsm\Samples\SampleWatch_b.sm1",
        r"S:\besa\Derivatives\docs\technical\workflows\Resets\Server\SingleResetWF.sm1",
        r"S:\besa\Derivatives\docs\technical\workflows\Resets\Server\SingleTradeLegResetsAndPaymentsManagerWF.sm1"]
    parsedModel = ParsedModel(fileList[-1])
    
    def simplePrintFunction(item, arg):
        genericPrintVisitor = GenericPrintVisitor(parsedModel)
        item.accept(genericPrintVisitor, arg)

    parsedModel.do(simplePrintFunction, None)

    # placed three sample languages - all incomplete.
    # python, javascript and smalltalk
    # need things like - qualified events (due to ports), action parsing for sending to ports and timeout parsing for timeouts
    # no need for codegen'ed file to contain user supplied code as these languages support class extension
    # python via mixin, javascript directly through prototype and smalltalk - can be done - but don't even know file format for straight code-gen.

            
    pythonVisitor = PythonVisitor(parsedModel)
    javascriptVisitor = JavascriptVisitor(parsedModel)
    smalltalkVisitor = SmalltalkVisitor(parsedModel)

    def visitorPassthroughFunction(item, visitor):
        item.accept(visitor, None)

    parsedModel.do(visitorPassthroughFunction, pythonVisitor)
    parsedModel.do(visitorPassthroughFunction, javascriptVisitor)
    parsedModel.do(visitorPassthroughFunction, smalltalkVisitor)

    stateTreeRoot = buildStateTree(parsedModel)
    print stateTreeRoot

    walkStateTree(stateTreeRoot, 1)