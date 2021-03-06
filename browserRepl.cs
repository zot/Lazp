if window? and (!global? or global == window)
  window.global = window
  window.Repl = root = {}
  Pretty = window.Pretty
else root = exports ? this

lastLine = null
input = null
write = null
envFrame = null

init = (inputField, output, defs)->
  write = (line)-> defs.innerHTML += line
  ReplCore.setWriter (line)->
    output.innerHTML += "<span>#{line}</span>"
    output.lastChild.scrollIntoView()
  ReplCore.setNext -> input.value = ''
  ReplCore.setHandler (ast, result, a, c, r, src)->
    if ast.leisureName? then defs.innerHTML += "#{markupDef(src, ast)}<br>"
    else if result
      output.innerHTML += "<span><b> #{lastLine} \u2192</b>\n  #{ReplCore.getType result}: #{Pretty.print result}</span>\n"
      output.lastElementChild.scrollIntoView()
    ReplCore.processResult result
  ReplCore.setResetFunc clearEnv
  input = inputField
  input.onkeypress = (e)->
    if (e.charCode || e.keyCode || e.which) == 13
      lastLine = input.value.replace(/\\/g, '\u03BB')
      ReplCore.processLine(lastLine)
  input.select()

reloadEnv = -> envFrame.contentWindow.location.reload()

clearOutput = ->
  o = document.getElementById('output')
  o.innerHTML = 'Click on <a id="stdDefsLink2" href="javascript:void(getStdDefs())">Standard Functions</a> to see Leisure\'s standard functions.\n\n'
  ReplCore.help()
  o.innerHTML += '\n'

clearEnv = (clearEnv)->
  document.getElementById('defs').innerHTML = ""
  env = document.getElementById('env')
  document.body.removeChild(env)
  newEnv = document.createElement('iframe')
  newEnv.id = 'env'
  newEnv.setAttribute("style", "display: none")
  newEnv.setAttribute("onload", "Repl.useIframe(this)")
  document.body.appendChild(newEnv)

useIframe = (envFr)->
  if (envFr)
    root.envFrame = envFrame = envFr
    env = envFrame.contentWindow
    env[i] = v for i, v of leisureFuncs
    Leisure.setEvalFunc env, env.eval
    env[i] = v for i, v of {Leisure: Leisure, ReplCore: ReplCore, Repl: Repl, leisureFuncs: {}, macros: {}}
    env.eval """
global = window;
setType = Leisure.setType;
setDataType = Leisure.setDataType;
define = Leisure.define;
defineMacro = Leisure.defineMacro;
defineToken = Leisure.defineToken;
processResult = Repl.processResult;
(function(){
var lll;

  global.leisureGetFuncs = function leisureGetFuncs() {
    return lll
  }
  global.leisureSetFuncs = function leisureSetFuncs(funcs) {
    lll = funcs
  }
  global.leisureAddFunc = function leisureAddFunc(func) {
    lll = Leisure.cons(func, lll)
  }
})()
"""
    clearOutput()

markupDef = (src, ast)->
  if src.match /^\s*#/ then src
  if (match = src.match Leisure.linePat)
    [matched, leading, name, defType] = match
    "<div><b>#{name}</b> #{defType} #{src.substring(matched.length)}</div>"
  else line

markupLines = (lines)-> lines.split('\n').map(markupDef).join('<br>')

handleFiles = (fileElement)->
  files = fileElement.files
  reader = new FileReader()
  reader.onerror = (evt)->
    window.e = evt
    alert('error' + evt)
    fileElement.value = null
  reader.onload = ->
    code = ReplCore.generateCode(files[0], reader.result, false, true)
    eval(code)
    fileElement.value = null
  reader.readAsText(files[0])
  input.select()

processResult = (result)->
  write("#{getType result}: #{P.print(result)}\n")
  ReplCore.processResult result

root.init = init
root.markupDef = markupDef
root.markupLines = markupLines
root.handleFiles = handleFiles
root.useIframe = useIframe
root.reloadEnv = reloadEnv
root.clearEnv = clearEnv
