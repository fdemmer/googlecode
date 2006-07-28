#!/usr/bin/env python
# -*- coding: iso-8859-1 -*-
# -----------------------------------------------------------------------
# xmlrpc.py - a small xmlrpc server in a plugin
# -----------------------------------------------------------------------
# maintainer: Florian Demmer <fdemmer@gmail.com> http://demmer.ipax.at
# 
# description:
# ...written for use with FreevoGenial which uses XMLRPC to add and 
# remove scheduled recordings in a remote Freevo installation. This was 
# necessary as the original recordserver XMLRPC server does not under-
# stand arrays as method arguments.
#
# setup/installation:
# * put xmlrpc.py into your freevo/plugins directory
# * add the following to your local_config.py
#   plugin.activate('xmlrpc')
#   XMLRPC_PORT=18010 # this is optional, 18010 is default
# ----------------------------------------------------------------------- */

import threading

import config
import plugin
import tv

from twisted.web import xmlrpc

"""plugin interface class"""
class PluginInterface(plugin.DaemonPlugin):
    def __init__(self):
        plugin.DaemonPlugin.__init__(self)
        server = PluginXmlrpc()
        _debug_('initialized.')

    def config(self):
        return [ ('XMLRPC_PORT', 18010, 'XMLRPC server port') ]

"""PluginXmlrpc main and thread class"""
class PluginXmlrpc(xmlrpc.XMLRPC, threading.Thread):
    def __init__(self):
        threading.Thread.__init__(self)
        self.start()

    """ xmlrpc server run thread"""
    def run(self):
        from twisted.internet import reactor
        from twisted.web import server
        reactor.listenTCP(config.XMLRPC_PORT, server.Site(self))
        _debug_('listening on port %s ...' % config.XMLRPC_PORT)
        reactor.run(installSignalHandlers=0)
        _debug_('stopped.')

    """convert a map/associative array into a Freevo-style TVProgram object"""
    def getTvProgramFromDictionary(self, dict):
        prog = tv.epg_types.TvProgram()
        prog.channel_id = dict["channel_id"]
        prog.title = dict["title"]
        prog.desc = dict["desc"]
        prog.sub_title = dict["sub_title"]
        prog.start = dict["start"]
        prog.stop = dict["stop"]
        prog.date = ""
        return prog

    # PUBLISHED XMLRPC METHODS BELOW HERE!

    def xmlrpc_scheduleRecording(self, prog=None):
        if not prog:
            return (FALSE, 'pluginxmlrpctest(): no prog')
        elif isinstance(prog, dict):
            prog = self.getTvProgramFromDictionary(prog)
        else:
            prog = unjellyFromXML(prog)

        msg = tv.record_client.scheduleRecording(prog)
        _debug_('scheduleRecording(): '+str(msg))

    def xmlrpc_removeScheduledRecording(self, prog=None):
        if not prog:
            return (FALSE, 'removeScheduledRecording(): no prog')
        elif isinstance(prog, dict):
            prog = self.getTvProgramFromDictionary(prog)
        else:
            prog = unjellyFromXML(prog)

        msg = tv.record_client.removeScheduledRecording(prog)
        _debug_('removeScheduledRecording(): '+str(msg))

    # END OF XMLRPC METHODS.
