#testCodeBuilder
from reader.parseStateProtoFile import *
import codegens.codebuilder
import codegens.pygen
import codegens.jsgen


if __name__ == "__main__":
    
    def generatePythonCode(parsedModel):
        lang = codegens.pygen.PythonLanguageGenerator()
        cb = codegens.codebuilder.CodeBuilder(parsedModel, lang)
        cb.build()

    def generateJavascriptCode(parsedModel):
        lang = codegens.jsgen.JavascriptLanguageGenerator()
        cb = codegens.codebuilder.CodeBuilder(parsedModel, lang)
        cb.build()

    def usage(defaultFileName):
        print """
    usage:
        testcodebuilder.py [-js,-py] -f stateMachineFileName.sm1 

        where either pass in -js to generate javascript
                          or -py to generate python code.

        the default filename for this testcode is %s                          
""" % (defaultFileName,)
        import sys
        sys.exit(2)

    def processArgs():
        import sys
        lang = None
        fileName = r"reader\testsample1.sm1"
        nextIs = None
        for arg in sys.argv:
            if nextIs == "SMFILENAME":
                fileName = arg
                nextIs = None
            if arg == "-js":
                lang = "JS"
            elif arg == "-py":
                lang = "PY"
            elif arg == "-f":
                nextIs = "SMFILENAME"
            pass
        if lang == None or fileName == None:
            usage(fileName)
            pass
        return lang, fileName

    lang, fileName = processArgs()    

    parsedModel = ParsedModel(fileName)

    generators = {}
    generators["PY"] = generatePythonCode
    generators["JS"] = generateJavascriptCode
    generators[lang](parsedModel)
    
