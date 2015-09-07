# CodeBuilder

class CodeBuilder(object):
    def __init__(self, model, language):
        self._Model = model
        self._Language = language
        self._Language.setModel(model)
        pass
    def build(self):
        self.createClass()
        pass
    def createClass(self):
        self.createClassHeader()
        self.createClassBody()
        self.createClassTrailer()
        pass
    def createClassHeader(self):
        self._Language.writeClassHeader(self._Model.header())
        pass
    def createClassBody(self):
        self._Language.writeStartClassBody()
        try:
            self.createStateMethods()
        finally:
            self._Language.writeEndClassBody()
            pass
        pass
    def createStateMethods(self):
        self._Model.stateList().do(self.createStateMethod, None)
    def createStateMethod(self, state, arg):
        self._Language.writeStateMethod(state)
    def createClassTrailer(self):
        self._Language.writeClassTrailer(self._Model.header())
        pass
    #end of CodeBuilder
    pass

    